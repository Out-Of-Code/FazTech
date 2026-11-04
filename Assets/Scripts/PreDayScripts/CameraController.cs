using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 MousePos;
    private Vector3 PreviousMousePos;
    private Vector3 WorldMousePos;
    private Vector3 NewWorldMousePos;
    private Vector3 MouseMoveDistance;
    private float MouseScrollAmount;
    public float MouseScrollSensitivity = 10;
    public float MinZoom = 0.1f;
    public float MaxZoom = 15f;
    void Update()
    {
        MouseScrollAmount = Input.GetAxis("Mouse ScrollWheel") * MouseScrollSensitivity;
        WorldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - MouseScrollAmount, MinZoom, MaxZoom);
        NewWorldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        WorldMousePos.z = 10;
        NewWorldMousePos.z = 10;
        Camera.main.transform.position += WorldMousePos-NewWorldMousePos;
        PreviousMousePos = MousePos;
        MousePos = Input.mousePosition * (2*Camera.main.orthographicSize/Screen.height);
        MousePos.z = 10;
        if (Input.GetMouseButtonDown(2))
        {
            PreviousMousePos = MousePos;
        }

        if (Input.GetMouseButton(2))
        {
            MouseMoveDistance = PreviousMousePos - MousePos;
            Camera.main.transform.position += MouseMoveDistance;
        }
    }
}
