using UnityEngine;
using UnityEngine.UI;

public class StateToggle : MonoBehaviour
{
    [SerializeField] private Image stateImage;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    public void Init(bool value)
    {
        stateImage.sprite = value ? onSprite : offSprite;
    }
}
