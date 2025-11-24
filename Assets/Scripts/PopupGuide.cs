using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupGuide : MonoBehaviour
{
    [SerializeField] private GameObject closeButton;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image image;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();

        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = Vector2.zero;

        closeButton.SetActive(false);
    }
    Sequence seq;
    public void Show(int index)
    {
        int offsetY = 200 * index;
        rectTransform.anchoredPosition = new Vector2(0, -50 + offsetY);

        seq = DOTween.Sequence();

        // 출력 모션
        seq.AppendCallback(() => { gameObject.SetActive(true); });
        seq.Append(canvasGroup.DOFade(1f, 0.5f));
        seq.Join(rectTransform.DOAnchorPos(new Vector2(0, -10 + offsetY), 0.5f));

        // 끄기 활성화
        seq.InsertCallback(1f, () => 
        {
            image.raycastTarget = true; 
            closeButton.SetActive(true); 
        });

        // 대기
        seq.Append(rectTransform.DOAnchorPos(new Vector2(0, offsetY), 10f));

        // 소멸 모션
        seq.Append(canvasGroup.DOFade(0f, 1f));
        seq.AppendCallback(() => { gameObject.SetActive(false); });
    }
    public void OnClick()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        seq.Kill();
        gameObject.SetActive(false);
    }
}
