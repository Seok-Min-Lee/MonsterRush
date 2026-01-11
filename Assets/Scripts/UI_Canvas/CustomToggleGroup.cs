using UnityEngine;
using UnityEngine.UI;

public class CustomToggleGroup : MonoBehaviour
{
    [SerializeField] CustomToggle[] weaponToggles;
    [SerializeField] CustomToggle magnetToggle;
    [SerializeField] Image healGauge;

    [SerializeField] SpriteRenderer magnetRenderer;
    public Image HealGauge => healGauge;

    private RectTransform rectTransform;

    private bool isMagnetVisible = true;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void OnClickMagnet()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        Init(!isMagnetVisible);
    }
    public void ChangeUI(bool isLeftHand)
    {
        if (isLeftHand)
        {
            rectTransform.anchorMin = new Vector2(1, 0);
            rectTransform.anchorMax = new Vector2(1, 0);
            rectTransform.pivot = new Vector2(1, 0);
            rectTransform.anchoredPosition = new Vector2(-75, 260);
        }
        else
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.anchoredPosition = new Vector2(275, -40);
        }
    }
    public void Init(bool isMagentVisible)
    {
        this.isMagnetVisible = isMagentVisible;

        magnetToggle.SetState(isMagnetVisible);
        magnetRenderer.enabled = isMagnetVisible;
    }
}
