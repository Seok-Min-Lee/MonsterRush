using UnityEngine;
using UnityEngine.UI;

public class Pagination : MonoBehaviour
{
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    private Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetValue(bool isOn)
    {
        image.sprite = isOn ? onSprite : offSprite;
    }
}
