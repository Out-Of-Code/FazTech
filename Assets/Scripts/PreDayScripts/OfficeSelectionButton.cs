using System;
using UnityEngine;
using UnityEngine.UI;

public class OfficeSelectionButton : MonoBehaviour
{
    public GameObject prefab;
    public Button Button;
    public RoomPlacement Placement;

    private void Start()
    {
        Button.onClick.AddListener(() => Placement.ChangeOffice(prefab));
    }
}