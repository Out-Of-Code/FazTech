using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomDatabase", menuName = "FazTech/Room Database")]
public class RoomDatabase : ScriptableObject
{
    public List<RoomInfo> Rooms = new List<RoomInfo>();

    public GameObject GetNightPrefab(string roomID)
    {
        RoomInfo room = Rooms.Find(r => r.name == roomID);

        if (room == null)
            return null;

        return room.NightPrefab;
    }
}