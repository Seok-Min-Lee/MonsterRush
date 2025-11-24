using DG.Tweening;
using TMPro;
using UnityEngine;

public class NormalWindow : MonoBehaviour
{
    [SerializeField] CanvasGroup alertCG;
    [SerializeField] TextMeshProUGUI alertText;

    private CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        alertCG.alpha = 0;
    }
    public void Show()
    {
        canvasGroup.DOFade(1f, 0.5f).OnComplete(() => 
        {
            canvasGroup.blocksRaycasts = true; 
        });
    }
    public void Hide()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0f, 0.5f);
    }
    public void Alert(string text)
    {
        alertText.text = text;

        Sequence seq = DOTween.Sequence();
        seq.Append(alertCG.DOFade(1f, 0.5f));
        seq.AppendInterval(2f);
        seq.Append(alertCG.DOFade(0f, 0.5f));
    }
}
