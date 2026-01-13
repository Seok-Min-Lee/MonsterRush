using TMPro;
using UnityEngine;

public class BuffBlock : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;

    public void SetValue(string value)
    {
        text.text = value;
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
