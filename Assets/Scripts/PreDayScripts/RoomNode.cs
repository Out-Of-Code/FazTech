using System;
using UnityEngine;
public enum ConnectionType
{
    Hallway,
    Vent,
    Duct
}
public class RoomNode : MonoBehaviour
{
    public ConnectionType Connection;
    public RoomNode ConnectedNode;
    
    private void Update()
    {
        //ConnectedNode.GetComponentInParent<Room>();
        if (ConnectedNode != null)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }
}
