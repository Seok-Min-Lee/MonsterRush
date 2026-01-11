using UnityEngine;

public class GaugeBar : MonoBehaviour
{
    [SerializeField] private Transform fill;

    private float maxValue;
    private float currentValue;

    public void Init(float maxValue, float currentValue, bool visible)
    {
        this.maxValue = maxValue;
        this.currentValue = currentValue;
        SetValue(currentValue);

        if (visible)
        {
            Show();
        }
        else         
        {
            Hide();
        }
    }
    public void SetValue(float value)
    {
        Show();

        currentValue = Mathf.Clamp(value, 0, maxValue);
        float fillAmount = currentValue / maxValue;
        fill.localScale = new Vector3(fillAmount, 1f, 1f);
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
