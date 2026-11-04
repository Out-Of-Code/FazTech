using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class RoomInventoryEntry
{
    public RoomInfo RoomData;
    public int Quantity;
}

public class RoomInventory : MonoBehaviour
{
    public List<RoomInventoryEntry> Rooms = new List<RoomInventoryEntry>();
    public Transform Content;
    public GameObject RoomEntryPrefab;
    public RectTransform InventoryPanel;
    public RoomPlacement Placement;
    private List<GameObject> Buttons = new List<GameObject>();
    void Start()
    {
        foreach (RoomInventoryEntry room in Rooms)
        {
            GameObject entry = Instantiate(RoomEntryPrefab, Content);
            
            RoomInventoryButton button = entry.AddComponent<RoomInventoryButton>();

            button.RoomData = room.RoomData;
            button.Button = entry.GetComponent<Button>();
            button.Text = entry.GetComponentInChildren<TMP_Text>();
            button.Inventory = this;
            
            TMP_Text text = entry.GetComponentInChildren<TMP_Text>();
            text.text = room.RoomData.DisplayName + " X" + room.Quantity;
            
            Buttons.Add(entry);
        }
    }
    public bool SelectRoom(RoomInventoryButton button)
    {
        if (button.RoomData == null)
            return false;

        RoomInventoryEntry room = Rooms.Find(r => r.RoomData == button.RoomData);

        if (room == null || room.Quantity <= 0)
            return false;

        room.Quantity--;
        button.Text.text = room.RoomData.DisplayName + " X" + room.Quantity;
        
        Debug.Log("Selected room: " + button.RoomData.DisplayName);
        return true;
        //button.gameObject.SetActive(room.Quantity > 0);
    }
    
    public bool IsPointerInsideInventory(Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            InventoryPanel,
            screenPosition
        );
    }
    public void ReturnRoom(RoomInventoryButton button)
    {
        RoomInventoryEntry room = Rooms.Find(r => r.RoomData == button.RoomData);

        if (room == null)
            return;

        room.Quantity++;
        button.Text.text = room.RoomData.DisplayName + " X" + room.Quantity;
        button.gameObject.SetActive(true);
    }

    public void ReturnRoom(RoomInfo roomData)
    {
        RoomInventoryEntry room = Rooms.Find(r => r.RoomData == roomData);
        
        if (room == null)
            return;
        
        room.Quantity++;

        GameObject button = Buttons.Find(b => b.GetComponent<RoomInventoryButton>().RoomData == roomData);
        if (button == null)
            return;
        button.GetComponent<RoomInventoryButton>().Text.text = room.RoomData.DisplayName + " X" + room.Quantity;
        button.gameObject.SetActive(true);
    }
}