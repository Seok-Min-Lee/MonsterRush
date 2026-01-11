using DG.Tweening;
using TMPro;
using UnityEngine;
public enum GameResult { Clear, Defeat, Challenge }
public class EndingWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI head;

    [SerializeField] private TextMeshProUGUI timeScoreText;
    [SerializeField] private TextMeshProUGUI levelScoreText;
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
        levelScoreText.text = $"{Player.Instance.Level}";
        killScoreText.text = $"{Player.Instance.killCount.ToString("#,##0")}";

        if (result != GameResult.Defeat)
        {
            head.text = "½Â¸®!!";
            head.color = Color.yellow;
        }
        else
        {
            head.text = "ÆÐ¹è..";
            head.color = new Color(0.125f, 0.125f, 0.125f, 1f);
        }

        continueButton.SetActive(result == GameResult.Clear);

        canvasGroup.alpha = 0f;
    }
    public void Show()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);
    }
}
