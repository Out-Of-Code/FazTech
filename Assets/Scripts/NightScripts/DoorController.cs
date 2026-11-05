using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DoorController : MonoBehaviour
{
    public bool IsClosed = false;

    [Header("Door Positions")]
    [SerializeField] private Transform Door;
    [SerializeField] private Transform OpenPosition;
    [SerializeField] private Transform ClosedPosition;

    [Header("Animation")]
    [SerializeField] private float MoveDuration = 0.5f;
    
    public UnityEvent OnOpen;
    public UnityEvent OnClose;
    public bool CanUse = true;
    private bool IsMoving = false;
    private Coroutine MoveCoroutine;

    public void ToggleDoor()
    {
        if (!CanUse || IsMoving)
            return;
        Debug.Log(gameObject.name + " is " +
                  (!IsClosed ? "CLOSED" : "OPEN"));
        IsMoving = true;
        IsClosed = !IsClosed;
        Vector3 TargetPosition = !IsClosed
            ? OpenPosition.position
            : ClosedPosition.position;

        MoveCoroutine = StartCoroutine(
            MoveDoor(TargetPosition));


        if (!IsClosed)
        {
            OnOpen?.Invoke();
        }
        else
        {
            OnClose?.Invoke();
        }
    }

    private IEnumerator MoveDoor(Vector3 TargetPosition)
    {
        Vector3 StartPosition = Door.position;
        float Elapsed = 0f;

        while (Elapsed < MoveDuration)
        {
            Elapsed += Time.deltaTime;
            float Progress = Mathf.Clamp01(Elapsed / MoveDuration);

            Door.position = Vector3.Lerp(
                StartPosition, TargetPosition, Progress);

            yield return null;
        }

        Door.position = TargetPosition;
        MoveCoroutine = null;
        IsMoving = false;
    }

    public void OpenDoor()
    {
        IsClosed = false;
        Vector3 TargetPosition = OpenPosition.position;
        MoveCoroutine = StartCoroutine(MoveDoor(TargetPosition));
        OnOpen?.Invoke();
        IsClosed = false;
    }
    public void DisableDoor()
    {
        CanUse = false;
    }
}