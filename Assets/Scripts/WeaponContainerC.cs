using UnityEngine;

public class WeaponContainerC : WeaponContainer<WeaponC>
{
    private void Awake()
    {
        this.Init();
    }
    public override void Init()
    {
        WeaponC weapon = Instantiate<WeaponC>(prefab, transform);
        weapon.gameObject.SetActive(false);

        weapons.Add(weapon);
    }
    public override void Add()
    {
        if (activeCount >= WEAPON_COUNT_MAX)
        {
            return;
        }

        if (activeCount == 0)
        {
            weapons[0].gameObject.SetActive(true);
        }
        else
        {
            weapons[0].Expand();
        }

        activeCount++;
    }
}
