using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour
{
    public UnityEvent OnClick;

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick.Invoke();
        }
    }
}