using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = "New Room", menuName = "FazTech/Room Data")]
public class RoomInfo : ScriptableObject
{
    public string DisplayName;
    [Space(20)]
    [InfoBox("The Pre-Day prefab assigned to the ScriptableObject.", EInfoBoxType.Normal)]
    public GameObject RoomPrefab;
    [Space(20)]
    [InfoBox("The Night prefab spawned when you transition from Pre-Day to Night.", EInfoBoxType.Normal)]
    public GameObject NightPrefab;
    public int StartingQuantity;
    
}