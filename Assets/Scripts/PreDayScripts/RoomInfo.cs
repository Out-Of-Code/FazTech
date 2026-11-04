using UnityEngine;


[CreateAssetMenu(fileName = "New Room", menuName = "FazTech/Room Data")]
public class RoomInfo : ScriptableObject
{
    public string DisplayName;
    public GameObject RoomPrefab;
    public int StartingQuantity;
}