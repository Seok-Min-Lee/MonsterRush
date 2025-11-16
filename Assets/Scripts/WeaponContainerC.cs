using UnityEngine;

public class WeaponContainerC : WeaponContainer<WeaponC>
{
    private bool isPoison = true;
    private void Awake()
    {
        this.Init();
    }
    public override void OnClickStateToggle()
    {
        base.OnClickStateToggle();

        isPoison = !isPoison;
        stateToggle.Init(isPoison);
        weapons[0].SwitchMode(isPoison);
    }
    public override void Init()
    {
        WeaponC weapon = Instantiate<WeaponC>(prefab, transform);
        weapon.gameObject.SetActive(false);

        weapons.Add(weapon);
    }
    public override void StrengthenFirst()
    {
        if (activeCount >= WEAPON_COUNT_MAX)
        {
            return;
        }

        AudioManager.Instance.PlaySFX(SoundKey.PlayerGetWeaponC);

        if (activeCount == 0)
        {
            weapons[0].gameObject.SetActive(true);

            stateToggle.Unlock();
            stateToggle.Init(isPoison);
            weapons[0].Init(isPoison);
        }
        else
        {
            weapons[0].Expand();
        }

        activeCount++;
    }
    public override void StrengthenSecond()
    {
        weapons[0].Strengthen();
    }
}
