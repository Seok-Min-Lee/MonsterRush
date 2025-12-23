using DG.Tweening;
using TMPro;
using UnityEngine;
public enum GameResult { Clear, Defeat, Challenge }
public class EndingWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI head;

    [SerializeField] private TextMeshProUGUI timeScoreText;
    [SerializeField] private TextMeshProUGUI LevelScoreText;
    [SerializeField] private TextMeshProUGUI killScoreText;

    [SerializeField] private GameObject continueButton;

    private CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnClickContinue()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
        GameCtrl.Instance.OnGameChallenge();
        gameObject.SetActive(false);
    }
    public void OnClickTitle()
    {
        GameCtrl.Instance.OnClickHome();
    }
    public void Init(GameResult result, float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        timeScoreText.text = $"{minutes:00}:{seconds:00}";
        LevelScoreText.text = $"{Player.Instance.Level}";
        killScoreText.text = $"{Player.Instance.killCount.ToString("#,##0")}";

        head.text = result != GameResult.Defeat ? "½Â¸®!!" : "ÆÐ¹è..";
        head.color = result != GameResult.Defeat ? Color.yellow : new Color(0.125f, 0.125f, 0.125f, 1f);
        continueButton.SetActive(result == GameResult.Clear);

        canvasGroup.alpha = 0f;
    }
    public void Show()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);
    }
}
