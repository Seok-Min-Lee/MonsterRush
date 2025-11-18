using UnityEngine;
using UnityEngine.UI;

public class StateToggle : MonoBehaviour
{
    [SerializeField] private Image stateImage;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite lockSprite;
    [SerializeField] private bool isLock;
    private void Start()
    {
        if (isLock)
        {
            GetComponent<Button>().interactable = false;
            stateImage.sprite = lockSprite;
        }
    }
    public void Init(bool value)
    {
        if (isLock)
        {
            return;
        }

        stateImage.sprite = value ? onSprite : offSprite;
    }
    public void Lock()
    {
        stateImage.sprite = lockSprite;

        GetComponent<Button>().interactable = false;
        isLock = true;
    }
    public void Unlock()
    {
        GetComponent<Button>().interactable = true;
        isLock = false;
    }
}
