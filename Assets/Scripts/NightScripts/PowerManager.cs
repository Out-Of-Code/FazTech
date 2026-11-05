using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PowerManager : MonoBehaviour
{
    [Header("Power Settings")]
    public float Power = 100f;
    public float MaxPower = 100f;

    [Header("Non-Linear Drain Configuration")]
    [Tooltip("Index 0 = 1 Bar (Idle), Index 1 = 2 Bars, ..., Index 5 = 6 Bars (Max Panic)")]
    [SerializeField] 
    private float[] DrainRatesPerSecond = new float[6] { 0.12f, 0.25f, 0.42f, 0.65f, 0.95f, 1.35f };

    [Header("Visuals")] 
    [Tooltip("Assign your 5 or 6 UI green/red usage block GameObjects here in order")]
    [SerializeField] private GameObject[] PowerPanels;

    public GameObject[] RegularLights;
    public GameObject BlackoutLights;
    [Space(100)]
    public TMP_Text PowerLevelText;
    public bool IsBlackout { get; private set; } = false;
    public UnityEvent OnBlackout;
    private int currentUsage = 1; // Starts at 1 for the office Fan

    void Start()
    {
        Power = MaxPower;
        UpdatePowerVisuals();
    }

    void Update()
    {
        // 1. Calculate non-linear drain based on current usage array index
        int drainIndex = Mathf.Clamp(currentUsage - 1, 0, DrainRatesPerSecond.Length - 1);
        float activeDrainRate = DrainRatesPerSecond[drainIndex];

        // 2. Apply frame-rate independent decay
        if (!IsBlackout)
        {
            Power -= activeDrainRate * Time.deltaTime;
            Power = Mathf.Max(Power, 0f);

            if (Power <= 0f)
            {
                TriggerBlackout();
            }
        }

        // 3. Update the UI text
        float powerPercentage = (MaxPower > 0) ? (Power / MaxPower) * 100f : 0f;
        PowerLevelText.text = $"{Mathf.CeilToInt(powerPercentage)}%"; // CeilToInt matches the OG game feel

        // 4. Update visual usage blocks
        UpdatePowerVisuals();
    }

    // Call this from your door/light/monitor scripts when activated
    public void AddUsage()
    {
        currentUsage = Mathf.Clamp(currentUsage + 1, 1, 6);
    }

    // Call this from your door/light/monitor scripts when deactivated
    public void RemoveUsage()
    {
        currentUsage = Mathf.Clamp(currentUsage - 1, 1, 6);
    }

    // Public method for your TJoC Fuse Room lever script to hook into
    public void RechargePower(float amount)
    {
        Power = Mathf.Clamp(Power + amount, 0f, MaxPower);
    }

    private void UpdatePowerVisuals()
    {
        if (PowerPanels == null || PowerPanels.Length == 0) return;

        // Ensure we don't light up more panels than we actually have assigned in the Inspector
        int panelsToActivate = Mathf.Clamp(currentUsage, 1, PowerPanels.Length);

        for (int i = 0; i < PowerPanels.Length; i++)
        {
            if (PowerPanels[i] != null)
            {
                PowerPanels[i].SetActive(i < panelsToActivate);
            }
        }
    }
    public void TriggerBlackout()
    {
        if (IsBlackout)
            return;

        IsBlackout = true;
        OnBlackout?.Invoke();

        foreach (GameObject p in RegularLights)
        {
            p.SetActive(false);
        }
        BlackoutLights.SetActive(true);
        RemoveUsage();
        Debug.Log("BLACKOUT! The office has lost power.");
    }
}