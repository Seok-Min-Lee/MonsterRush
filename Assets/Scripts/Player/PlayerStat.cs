using TMPro;
using UnityEngine;

[System.Serializable]
public class PlayerStat
{
    public enum Type
    {
        Level,
        Kill,
        Heal,
        Strength,
        Speed,
        Magnet,
        WeaponA,
        WeaponB,
        WeaponC,
        WeaponD,
        Hp,
        HpMax,
        Exp,
        ExpMax
    }
    [SerializeField] private Type type;
    [SerializeField] private TMP_Text TextUI;
    private int value;
    private int maxValue;
    public int Value => value;
    public void Init(int value, int maxValue)
    {
        this.value = value;
        this.maxValue = maxValue;

        UpdateUI();
    }
    public void SetValue(int value)
    {
        this.value = Mathf.Min(value, maxValue);

        UpdateUI();
    }
    public void SetMaxValue(int value)
    {
        maxValue = value;

        UpdateUI();
    }
    public void IncreaseValue(int value = 1)
    {
        this.value = Mathf.Min(this.value + value, maxValue);

        UpdateUI();
    }
    private void UpdateUI()
    {
        if (TextUI != null)
        {
            TextUI.text = value == maxValue ? "MAX" : value.ToString();
        }
    }
}
