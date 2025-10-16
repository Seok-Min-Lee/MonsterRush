using TMPro;
using UnityEngine;

public class GameCtrl : MonoBehaviour
{
    public static GameCtrl Instance;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject pauseWindow;
    [SerializeField] private GameObject rewardWindow;

    private void Start()
    {
        Instance = this;

        timeText.text = "00:00";
    }
    private void Update()
    {
        float time = Time.time;
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);

        timeText.text = $"{minutes:00}:{seconds:00}";
    }
    public void OnClickPause()
    {
        Time.timeScale = 0f;
        pauseWindow.SetActive(true);
    }
    public void OnClickResume()
    {
        pauseWindow.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OnClickHome()
    {
        Debug.Log("OnClickHome");
    }
    public void OnLevelUp()
    {
        Time.timeScale = 0f;
        rewardWindow.SetActive(true);
    }
    public void OnClickReward()
    {
        rewardWindow.SetActive(false);
        Time.timeScale = 1f;
    }
}
