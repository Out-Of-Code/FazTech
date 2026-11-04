using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class SimpleFPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Gravity")]
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;
    
    [Header("Interaction")]
    public float InteractDistance = 3;

    
    [Header("Misc.")]
    public bool canMove = true;
    public float lookThreshold = 0.85f;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation;
    
    public UnityEvent FreddyMove;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. When the key is first pressed down, freeze the camera and free the mouse
        if (Input.GetKeyDown(KeyCode.Q))
        {
            canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 2. When the key is released, lock the mouse back to center and unfreeze look
        if (Input.GetKeyUp(KeyCode.Q))
        {
            canMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            FreddyMove.Invoke();
        }
        if (!canMove)
            return;

        HandleMouseLook();
        HandleMovement();
    }


    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Up/down camera rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Left/right player rotation
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        // Grounded check
        isGrounded = controller.isGrounded;

        // Small downward force to stay grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Ceiling check
        // Stops upward velocity when hitting a roof
        if ((controller.collisionFlags & CollisionFlags.Above) != 0 && velocity.y > 0)
        {
            velocity.y = 0f;
        }

        // Movement input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move =
            (transform.right * moveX + transform.forward * moveZ).normalized;

        controller.Move(move * (moveSpeed * Time.deltaTime));

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply vertical movement
        controller.Move(velocity * Time.deltaTime);
    }
}