using UnityEngine;
using UnityEngine.Events;

public class OfficeLight : MonoBehaviour
{
    public Light TargetLight;
    public UnityEvent OnLightSetOn;
    public UnityEvent OnLightSetOff;
    public bool CanUse = true;

    public bool IsOn { get; private set; }

    public void ToggleLight()
    {
        if (CanUse)
        {
            IsOn = !IsOn;
            TargetLight.gameObject.SetActive(IsOn);
            if (IsOn)
            {
                OnLightSetOn.Invoke();
            }
            else
            {
                OnLightSetOff.Invoke();
            }
        }
    }

    public void TurnOff()
    {
        if (!IsOn)
            return;

        IsOn = false;
        TargetLight.gameObject.SetActive(false);
        OnLightSetOff.Invoke();
    }

    public void DisableLight()
    {
        CanUse = false;
    }
}