using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject NightSelectPanel;

    [Header("Night Selection")]
    [SerializeField] private TMP_Text NightNumberText;

    private int SelectedNight = 1;

    public void OpenNightSelect()
    {
        MainMenuPanel.SetActive(false);
        NightSelectPanel.SetActive(true);
        UpdateNightText();
    }

    public void BackToMainMenu()
    {
        NightSelectPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void PreviousNight()
    {
        SelectedNight = Mathf.Max(0, SelectedNight - 1);
        UpdateNightText();
    }

    public void NextNight()
    {
        SelectedNight = Mathf.Min(10, SelectedNight + 1);
        UpdateNightText();
    }

    private void UpdateNightText()
    {
        NightNumberText.text = "NIGHT " + SelectedNight;
    }
    public void ConfirmNight()
    {
        PlayerPrefs.SetInt("SelectedNight", SelectedNight);
        SceneManager.LoadScene("Main");
    }
}