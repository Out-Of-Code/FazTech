using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement; 

[Serializable]
public class GameSave
{
    public PizzeriaSave Pizzeria = new PizzeriaSave();

    // Future:
    // public int Money;
    // public List<string> Unlocks;
    // public ...
}

[Serializable]
public class PizzeriaSave
{
    public List<SavedRoom> Rooms = new List<SavedRoom>();
}

[Serializable]
public class SavedRoom
{
    public string RoomID;
    public string InstanceID;
    public Vector3 Position;
    public Quaternion Rotation;
    public List<SavedNodeConnection> Connections = new();
}

[Serializable]
public class SavedNodeConnection
{
    public int NodeIndex;
    public string ConnectedRoomInstanceID;
    public int ConnectedNodeIndex;
    public ConnectionType ConnectionType;
}

public class SaveSystem : MonoBehaviour
{
    public GameSave Save = new GameSave();
    public void SavePizzeria(RoomPlacement placement)
    {
        print("Saved");
        Save.Pizzeria.Rooms.Clear();
        Debug.Log($"Rooms available to save: {placement.AllRooms.Count}");
        foreach (Room room in placement.AllRooms)
        {
            SavedRoom roomSave = new SavedRoom();
            
            roomSave.RoomID = room.RoomTransform.GetComponent<RoomInstance>().RoomData.name;
            roomSave.InstanceID = room.InstanceID;
            //we're just gonna ignore the possible 'System.NullReferenceException'
            roomSave.Position = room.RoomTransform.position;
            roomSave.Rotation = room.RoomTransform.rotation;
            foreach (RoomNode node in room.Nodes)
            {
                if (node.ConnectedNode == null)
                    continue;
                Room DestRoom = placement.AllRooms.FirstOrDefault(r =>
                    r.Nodes.Contains(node.ConnectedNode));

                if (DestRoom == null)
                    continue;

                int currentRoomIndex = placement.AllRooms.IndexOf(room);
                int destinationRoomIndex = placement.AllRooms.IndexOf(DestRoom);

                if (currentRoomIndex > destinationRoomIndex)
                    continue;

                int currentNodeIndex = Array.IndexOf(room.Nodes, node);
                int destinationNodeIndex = Array.IndexOf(DestRoom.Nodes, node.ConnectedNode);

                // If both nodes belong to the same room, avoid saving the pair twice.
                if (currentRoomIndex == destinationRoomIndex &&
                    currentNodeIndex > destinationNodeIndex)
                    continue;
                SavedNodeConnection nodeSave = new SavedNodeConnection();
                nodeSave.NodeIndex = currentNodeIndex;
                nodeSave.ConnectedRoomInstanceID = DestRoom.InstanceID;
                nodeSave.ConnectedNodeIndex = destinationNodeIndex;
                nodeSave.ConnectionType = node.Connection;
                roomSave.Connections.Add(nodeSave);
            }
            Debug.Log($"Saving {roomSave.RoomID} with ID: {roomSave.InstanceID}");
            Save.Pizzeria.Rooms.Add(roomSave);
        }
        string json = JsonUtility.ToJson(Save, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "playerSave.json"), json);
        Debug.Log(Application.persistentDataPath);
        SceneManager.LoadScene("Night");
    }
}
