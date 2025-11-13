using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class HitHealText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    public void Init(string value, Color color, Vector2 position, Vector2 target, Transform parent)
    {
        gameObject.SetActive(true);

        transform.parent = parent;

        text.text = value;
        text.color = color;
        text.rectTransform.anchoredPosition = position;
        text.rectTransform.DOAnchorPos(target, 0.5f).OnComplete(() => 
        {
            UIContainer.Instance.Push(this);
            gameObject.SetActive(false);
        });
    }
}
