using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;
public class NightManager : MonoBehaviour
{
    [Header("Night Settings")]
    [SerializeField] private float NightDuration = 360f;
    public int SelectedNight = 1;

    [Header("Events")]
    public UnityEvent OnNightWon;
    
    [Header("Office Clock")]
    [SerializeField] private TMP_Text ClockText;
    
    [Header("End Screen")]
    [SerializeField] private GameObject EndScreenPanel;
    [SerializeField] private TMP_Text EndTitleText;
    [SerializeField] private TMP_Text EndMessageText;
    
    [Header("Misc.")]
    public SimpleFPSController Player;
    
    private float TimeRemaining;
    private bool NightOver = false;

    void Awake()
    {
        SelectedNight = PlayerPrefs.GetInt("SelectedNight", 1);
        TimeRemaining = NightDuration;
    }

    void Update()
    {
        if (NightOver)
            return;

        TimeRemaining -= Time.deltaTime;
        float Progress = 1f - (TimeRemaining / NightDuration);
        int HoursPassed = Mathf.Min(6, Mathf.FloorToInt(Progress * 6));

        int Hour = (12 + HoursPassed) % 12;

        if (Hour == 0)
            Hour = 12;

        ClockText.text = Hour + " AM";

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            NightOver = true;
            Debug.Log("6 AM! YOU SURVIVED!");
            ShowWinScreen();
            OnNightWon.Invoke();
        }
    }

    public float GetTimeRemaining()
    {
        return TimeRemaining;
    }

    public void EndNight()
    {
        NightOver = true;
    }

    public int GetAILevel()
    {
        if (SelectedNight == 0)
            return 0;

        if (SelectedNight == 1)
            return 1;

        if (SelectedNight == 2)
            return 2;

        if (SelectedNight >= 3 && SelectedNight <= 9)
            return (SelectedNight - 1) * 2;

        if (SelectedNight == 10)
            return 20;

        return 1;
    }
    public void ShowWinScreen()
    {
        NightOver = true;
        EndScreenPanel.SetActive(true);
        EndTitleText.text = "YOU SURVIVED";
        EndMessageText.text = "6 AM";
    }
    public void ShowDeathScreen()
    {
        NightOver = true;
        EndScreenPanel.SetActive(true);
        EndTitleText.text = "GAME OVER";
        EndMessageText.text = "YOU DIED";
        Player.canMove = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
