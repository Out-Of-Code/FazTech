using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class Room
{
    public Transform RoomTransform;
    public RoomNode[] Nodes;
}
[System.Serializable]
public class RoomTransformBackup
{
    public Transform Room;
    public Vector3 Position;
    public Quaternion Rotation;
}

public class RoomPlacement : MonoBehaviour
{
    public Room RoomSelected;
    public List<Room> AllRooms = new List<Room>();
    public LineRenderer ConnectionLine;
    public RoomInventory Inventory;
    public List<GameObject> Offices = new List<GameObject>();
    public GameObject OfficeEntryPrefab;
    public Transform OfficeContent;
    public GameObject OfficeMenu;
    public Vector2 OfficeMenuOffset = new Vector2(20, 20);
    private RoomNode SelectedNode;
    private Vector3 DragOffset;
    private Vector3 WorldMousePos;
    private List<Transform> DragGroup;

    void Start()
    {
        RoomSelected = new Room();
        CreateOfficeButtons();
    }

    void Update()
    {
        Vector3 MousePos = Input.mousePosition;
        MousePos.z = 10;
        WorldMousePos = Camera.main.ScreenToWorldPoint(MousePos);
        WorldMousePos.z = 0;
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            print("Mouse Down");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

            RoomNode[] clickedNodes = new RoomNode[hits.Length];
            int nodeCount = 0;

            foreach (RaycastHit hit in hits)
            {
                RoomNode node = hit.transform.GetComponent<RoomNode>();

                if (node != null && node.ConnectedNode != null)
                {
                    RoomNode connectedNode = node.ConnectedNode;

                    bool nodesOverlap = Physics.ComputePenetration(
                        node.GetComponent<Collider>(),
                        node.transform.position,
                        node.transform.rotation,
                        connectedNode.GetComponent<Collider>(),
                        connectedNode.transform.position,
                        connectedNode.transform.rotation,
                        out Vector3 direction,
                        out float distance
                    );

                    if (nodesOverlap)
                    {
                        node.ConnectedNode = null;
                        connectedNode.ConnectedNode = null;

                        print("Disconnected " + node.name + " from " + connectedNode.name);
                        return;
                    }
                }
                else if (node != null)
                {
                    clickedNodes[nodeCount] = node;
                    nodeCount++;
                }
            }

            if (nodeCount >= 2)
            {
                RoomNode firstNode = clickedNodes[0];
                RoomNode secondNode = null;

                for (int i = 1; i < nodeCount; i++)
                {
                    if (clickedNodes[i].Connection == firstNode.Connection &&
                        clickedNodes[i].transform.parent != firstNode.transform.parent)
                    {
                        secondNode = clickedNodes[i];
                        break;
                    }
                }

                if (secondNode != null)
                {
                    ConnectNodes(firstNode, secondNode);
                    print("Automatically connected " + firstNode.name + " to " + secondNode.name);
                }
                else
                {
                    print("No compatible overlapping nodes found.");
                }
            }
            else if (nodeCount == 1)
            {
                SelectedNode = clickedNodes[0];
                ConnectionLine.gameObject.SetActive(true);
            }
            OfficeMenu.SetActive(false);
            if (nodeCount == 0 && hits.Length > 0)
            {
                RoomSelected.RoomTransform = hits[0].transform.parent;
                if (RoomSelected.RoomTransform.gameObject.name.Contains("Office"))
                {
                    OpenOfficeMenu(RoomSelected.RoomTransform.gameObject);
                }
                else
                {
                    RoomSelected.Nodes = RoomSelected.RoomTransform.GetComponentsInChildren<RoomNode>();

                    DragGroup = GetConnectedRooms(RoomSelected.RoomTransform);

                    DragOffset = RoomSelected.RoomTransform.position - WorldMousePos;
                }
            }
        }

        if (SelectedNode != null && Input.GetMouseButton(0))
        {
            ConnectionLine.SetPosition(0, SelectedNode.transform.position);
            ConnectionLine.SetPosition(1, WorldMousePos);
        }
        else if (RoomSelected.RoomTransform != null &&
                 DragGroup != null &&
                 Input.GetMouseButton(0))
        {
            if (!DragGroup.Exists(room => room.name.Contains("Office")))
            {
                DragGroupTowardMouse();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (RoomSelected.RoomTransform != null &&
                Inventory.IsPointerInsideInventory(Input.mousePosition))
            {
                if (DragGroup != null && !DragGroup.Exists(r => r.name.Contains("Office")))
                {
                    ReturnGroupToInventory(DragGroup);
                }
            }
            
            RoomSelected.RoomTransform = null;
            RoomSelected.Nodes = null;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (SelectedNode != null &&
                Physics.Raycast(ray, out hit, 100f))
            {
                RoomNode targetNode = hit.transform.GetComponent<RoomNode>();

                if (targetNode != null)
                {
                    ConnectNodes(SelectedNode, targetNode);
                }
            }

            DragGroup = null;
            ConnectionLine.gameObject.SetActive(false);
            SelectedNode = null;
        }

    }

    List<Transform> GetConnectedRooms(Transform startingRoom)
    {
        List<Transform> visitedRooms = new List<Transform>();
        Queue<Transform> roomsToCheck = new Queue<Transform>();

        roomsToCheck.Enqueue(startingRoom);

        while (roomsToCheck.Count > 0)
        {
            Transform currentRoom = roomsToCheck.Dequeue();

            if (visitedRooms.Contains(currentRoom))
                continue;

            visitedRooms.Add(currentRoom);

            RoomNode[] nodes = currentRoom.GetComponentsInChildren<RoomNode>();

            foreach (RoomNode node in nodes)
            {
                if (node.ConnectedNode != null)
                {
                    Transform connectedRoom = node.ConnectedNode.transform.parent;

                    if (!visitedRooms.Contains(connectedRoom))
                        roomsToCheck.Enqueue(connectedRoom);
                }
            }
        }

        return visitedRooms;
    }

    List<RoomTransformBackup> SaveGroupTransforms(List<Transform> group)
    {
        List<RoomTransformBackup> backups = new List<RoomTransformBackup>();

        foreach (Transform room in group)
        {
            backups.Add(new RoomTransformBackup
            {
                Room = room,
                Position = room.position,
                Rotation = room.rotation
            });
        }

        return backups;
    }

    void RestoreGroupTransforms(List<RoomTransformBackup> backups)
    {
        foreach (RoomTransformBackup backup in backups)
        {
            backup.Room.position = backup.Position;
            backup.Room.rotation = backup.Rotation;
        }
    }

    bool HasRoomCollision(List<Transform> movingGroup)
    {
        foreach (Transform movingRoom in movingGroup)
        {
            Collider[] movingColliders = movingRoom.GetComponentsInChildren<Collider>();

            foreach (Room room in AllRooms)
            {
                if (room.RoomTransform == null ||
                    movingGroup.Contains(room.RoomTransform))
                    continue;

                Collider[] stationaryColliders =
                    room.RoomTransform.GetComponentsInChildren<Collider>();

                foreach (Collider movingCollider in movingColliders)
                {
                    if (movingCollider.transform.parent != movingRoom ||
                        movingCollider.gameObject.name != "Collider")
                        continue;

                    foreach (Collider stationaryCollider in stationaryColliders)
                    {
                        if (stationaryCollider.transform.parent != room.RoomTransform ||
                            stationaryCollider.gameObject.name != "Collider")
                            continue;

                        bool overlapping = Physics.ComputePenetration(
                            movingCollider,
                            movingCollider.transform.position,
                            movingCollider.transform.rotation,
                            stationaryCollider,
                            stationaryCollider.transform.position,
                            stationaryCollider.transform.rotation,
                            out Vector3 direction,
                            out float distance
                        );

                        if (overlapping)
                            return true;
                    }
                }
            }
        }

        return false;
    }

    void DragGroupTowardMouse()
    {
        Vector3 targetPosition = DragOffset + WorldMousePos;
        Vector3 movement = targetPosition - RoomSelected.RoomTransform.position;

        // Divide movement into small steps to prevent passing through rooms.
        int steps = Mathf.Max(1, Mathf.CeilToInt(movement.magnitude / 0.05f));
        Vector3 step = movement / steps;

        for (int i = 0; i < steps; i++)
        {
            // First try moving diagonally.
            if (TryMoveGroup(step))
                continue;

            // If blocked, try moving along each axis separately.
            Vector3 horizontal = new Vector3(step.x, 0, 0);
            Vector3 vertical = new Vector3(0, step.y, 0);

            if (Mathf.Abs(step.x) >= Mathf.Abs(step.y))
            {
                TryMoveGroup(horizontal);
                TryMoveGroup(vertical);
            }
            else
            {
                TryMoveGroup(vertical);
                TryMoveGroup(horizontal);
            }
        }
    }

    bool TryMoveGroup(Vector3 movement)
    {
        if (movement.sqrMagnitude < 0.000001f)
            return false;

        foreach (Transform room in DragGroup)
        {
            room.position += movement;
        }

        if (HasRoomCollision(DragGroup))
        {
            foreach (Transform room in DragGroup)
            {
                room.position -= movement;
            }

            return false;
        }

        return true;
    }

    void ConnectNodes(RoomNode firstNode, RoomNode secondNode)
    {
        if (firstNode == secondNode ||
            firstNode.ConnectedNode != null ||
            secondNode.ConnectedNode != null ||
            firstNode.Connection != secondNode.Connection ||
            firstNode.transform.parent == secondNode.transform.parent)
            return;

        List<Transform> selectedGroup = GetConnectedRooms(firstNode.transform.parent);
        List<Transform> targetGroup = GetConnectedRooms(secondNode.transform.parent);

        bool sameGroup = selectedGroup.Contains(secondNode.transform.parent);

        if (sameGroup)
        {
            bool nodesOverlap = Physics.ComputePenetration(
                firstNode.GetComponent<Collider>(),
                firstNode.transform.position,
                firstNode.transform.rotation,
                secondNode.GetComponent<Collider>(),
                secondNode.transform.position,
                secondNode.transform.rotation,
                out Vector3 direction,
                out float distance
            );

            if (nodesOverlap)
            {
                firstNode.ConnectedNode = secondNode;
                secondNode.ConnectedNode = firstNode;
                print("Connected overlapping nodes in the same group.");
            }
            else
            {
                print("Cannot connect nodes in the same group unless they overlap.");
            }

            return;
        }

        bool selectedHasOffice = selectedGroup.Exists(room => room.name.Contains("Office")
        );

        List<Transform> movingGroup = selectedHasOffice ? targetGroup : selectedGroup;

        RoomNode movingNode = selectedHasOffice ? secondNode : firstNode;
        RoomNode stationaryNode = selectedHasOffice ? firstNode : secondNode;

        List<RoomTransformBackup> backups = SaveGroupTransforms(movingGroup);

        float angle = Vector3.SignedAngle(
            movingNode.transform.right,
            -stationaryNode.transform.right,
            Vector3.forward
        );

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector3 pivot = movingNode.transform.parent.position;

        foreach (Transform room in movingGroup)
        {
            room.position = pivot + rotation * (room.position - pivot);
            room.rotation = rotation * room.rotation;
        }

        Vector3 offset = stationaryNode.transform.position - movingNode.transform.position;

        foreach (Transform room in movingGroup)
        {
            room.position += offset;
        }

        if (HasRoomCollision(movingGroup))
        {
            RestoreGroupTransforms(backups);
            print("Connection rejected: room collision.");
            return;
        }

        firstNode.ConnectedNode = secondNode;
        secondNode.ConnectedNode = firstNode;

        print("Rooms successfully connected.");
    }

    public void RegisterRoom(GameObject roomObject)
    {
        Room newRoom = new Room();

        newRoom.RoomTransform = roomObject.transform;
        newRoom.Nodes = roomObject.GetComponentsInChildren<RoomNode>();

        AllRooms.Add(newRoom);
    }
    public GameObject SpawnRoom(RoomInfo roomData, Vector3 position)
    {
        GameObject newRoom = Instantiate(
            roomData.RoomPrefab,
            position,
            Quaternion.identity
        );

        RegisterRoom(newRoom);

        return newRoom;
    }
    public Vector3 GetMouseWorldPosition()
    {
        return WorldMousePos;
    }
    public void StartDraggingSpawnedRoom(GameObject roomObject)
    {
        RoomSelected.RoomTransform = roomObject.transform;
        RoomSelected.Nodes = roomObject.GetComponentsInChildren<RoomNode>();

        DragGroup = new List<Transform> { roomObject.transform };

        DragOffset = roomObject.transform.position - WorldMousePos;
    }
    public void RemoveSpawnedRoom(GameObject roomObject)
    {
        if (roomObject == null)
            return;

        AllRooms.RemoveAll(room => room.RoomTransform == roomObject.transform);

        if (RoomSelected.RoomTransform == roomObject.transform)
        {
            RoomSelected.RoomTransform = null;
            RoomSelected.Nodes = null;
        }

        DragGroup = null;
        RoomSelected.RoomTransform = null;
        RoomSelected.Nodes = null;

        Destroy(roomObject);
    }


    void ReturnGroupToInventory(List<Transform> Rooms)
    {
        foreach (Transform room in Rooms)
        {
            RoomInstance roomInstance = room.GetComponent<RoomInstance>();

            if (roomInstance)
            {
                Inventory.ReturnRoom(roomInstance.RoomData);
            }
            else
            {
                Debug.LogError("No room instance on room " + room.name + "!");
            }
            AllRooms.RemoveAll(r => r.RoomTransform == room);
            Destroy(room.gameObject);
   
        }
    }

    void IsolateOffice()
    {
        Room room = AllRooms.Find(r => r.RoomTransform.name.Contains("Office"));
        if (room == null || room.RoomTransform == null)
            return;
        GameObject Office = room.RoomTransform.gameObject;
        foreach (RoomNode node in Office.GetComponentsInChildren<RoomNode>())
        {
            if (node.ConnectedNode != null)
            {
                node.ConnectedNode.ConnectedNode = null;
                node.ConnectedNode = null;
            }
        }
    }

    public void ChangeOffice(GameObject NewOffice)
    {
        Room room = AllRooms.Find(r => r.RoomTransform.name.Contains("Office"));
        if (room == null || room.RoomTransform == null)
            return;
        GameObject OldOffice = room.RoomTransform.gameObject;
        IsolateOffice();
        RemoveSpawnedRoom(OldOffice);
        GameObject spawnedOffice = Instantiate(NewOffice, new Vector3(0, 0, 0), Quaternion.identity);
        RegisterRoom(spawnedOffice);
        MoveOverlappingGroups(spawnedOffice);
    }

    void MoveOverlappingGroups(GameObject room)
    {
        List<Transform> MovedRooms = new List<Transform>();
        foreach (Room _room in AllRooms)
        {
            if (_room.RoomTransform == null || _room.RoomTransform == room.transform || MovedRooms.Contains(_room.RoomTransform))
                continue;
            bool IsColliding = RoomsCollide(room.gameObject, _room.RoomTransform.gameObject);
            if (IsColliding)
            {
                List<Transform> Connected_roomGroup = GetConnectedRooms(_room.RoomTransform);
                foreach (Transform transform in Connected_roomGroup)
                {
                    MovedRooms.Add(transform);
                }
                if (!FindAvailablePositions(Connected_roomGroup))
                {
                    Debug.LogError("Couldn't move a room group out of the way of the new Office!");
                }
            }
        }
    }

    bool TryPlaceGroupAtOffset(List<Transform> group, Vector3 offset)
    {
        bool doesWork = true;
        foreach (Transform room in group)
        {
            room.position += offset;
        }
        if (HasRoomCollision(group))
        {
            doesWork = false;
        }
        if (doesWork == true)
        {
            return true;
        }
        foreach (Transform room in group) 
            room.position -= offset;
        return false;
       
    }
    bool RoomsCollide(GameObject roomOne, GameObject roomTwo)
    {
        if (roomOne == null || roomTwo == null)
            return false;
        Collider[] movingColliders = roomOne.GetComponentsInChildren<Collider>();

        Collider[] stationaryColliders =
            roomTwo.transform.GetComponentsInChildren<Collider>();

        foreach (Collider movingCollider in movingColliders)
        {
            if (movingCollider.transform.parent != roomOne ||
                movingCollider.gameObject.name != "Collider")
                continue;

            foreach (Collider stationaryCollider in stationaryColliders)
            {
                if (stationaryCollider.transform.parent != roomTwo.transform ||
                    stationaryCollider.gameObject.name != "Collider")
                    continue;

                bool overlapping = Physics.ComputePenetration(
                    movingCollider,
                    movingCollider.transform.position,
                    movingCollider.transform.rotation,
                    stationaryCollider,
                    stationaryCollider.transform.position,
                    stationaryCollider.transform.rotation,
                    out Vector3 direction,
                    out float distance
                );

                if (overlapping)
                    return true;
            }
        }
        return false;
    }

    bool FindAvailablePositions(List<Transform> group)
    {
        int loops = 1;
        while (loops <= 150)
        {
            for (int x = -loops; x <= loops; x++)
            {
                for (int y = -loops; y <= loops; y++)
                {
                    if (Mathf.Abs(x) != loops && Mathf.Abs(y) != loops)
                        continue;
                    if (TryPlaceGroupAtOffset(group, new Vector3(x, y, 0)))
                    {
                        return true;
                    }
                }
            }

            loops += 1;
        }
        Debug.LogError("No available positions within 150 units!");
        return false;
    }
    
    void CreateOfficeButtons()
    {
        
        foreach (GameObject Office in Offices)
        {
            GameObject entry = Instantiate(OfficeEntryPrefab, OfficeContent);
        
            OfficeSelectionButton button = entry.AddComponent<OfficeSelectionButton>();
            
            button.Button = entry.GetComponent<Button>();
            button.prefab = Office;
            //button.Text = entry.GetComponentInChildren<TMP_Text>();
            button.Placement = this;
        
            TMP_Text text = entry.GetComponentInChildren<TMP_Text>();
            text.text = Office.name;
        
            //Buttons.Add(entry);
        }
    }

    void OpenOfficeMenu(GameObject office)
    {
        Vector2 Position = Camera.main.WorldToScreenPoint(office.transform.position);
        Position += OfficeMenuOffset;
        OfficeMenu.transform.position = Position;
        OfficeMenu.SetActive(true);
    }
}