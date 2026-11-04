using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class RoomInventoryButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RoomInfo RoomData;
    public Button Button;
    public TMP_Text Text;
    public RoomInventory Inventory;
    private bool HasLeftInventory = false;
    private GameObject SpawnedRoom;
    private bool HasReservedRoom = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        HasReservedRoom = Inventory.SelectRoom(this);

        if (HasReservedRoom)
            Debug.Log("Started dragging: " + RoomData.DisplayName);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!HasReservedRoom)
            return;
        if (!HasLeftInventory && !Inventory.IsPointerInsideInventory(eventData.position))
        {
            HasLeftInventory = true;
            SpawnedRoom = Inventory.Placement.SpawnRoom(
                RoomData,
                Inventory.Placement.GetMouseWorldPosition()
            );
            Inventory.Placement.StartDraggingSpawnedRoom(SpawnedRoom);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (HasReservedRoom && Inventory.IsPointerInsideInventory(eventData.position))
        {
            if (SpawnedRoom != null)
            {
                Inventory.Placement.RemoveSpawnedRoom(SpawnedRoom);
                SpawnedRoom = null;
            }

            Inventory.ReturnRoom(this);
        }
        else if (HasReservedRoom)
        {
            RoomInventoryEntry room = Inventory.Rooms.Find(
                r => r.RoomData == RoomData
            );

            if (room != null && room.Quantity <= 0)
                gameObject.SetActive(false);
        }
        HasReservedRoom = false;
        HasLeftInventory = false;
    }
}