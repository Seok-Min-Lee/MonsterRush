using UnityEngine;

public class WeaponContainerC : WeaponContainer<WeaponC>
{
    private bool isVisible = true;
    private void Awake()
    {
        this.Init();
        stateToggle.Init(!isVisible);
    }
    public override void OnClickStateToggle()
    {
        base.OnClickStateToggle();

        isVisible = !isVisible;
        stateToggle.Init(!isVisible);

        weapons[0].TextureRenderer.enabled = isVisible;
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
            weapons[0].TextureRenderer.enabled = isVisible;
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
