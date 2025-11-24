using UnityEngine;

public class WeaponContainerC : WeaponContainer<WeaponC>
{
    private WeaponC.State state = WeaponC.State.Poison;

    [Header("Debug")]
    [SerializeField] private int areaLevel = 0;
    [SerializeField] private int effectLevel = 0;
    private void Awake()
    {
        this.Init();
    }
    public override void OnClickStateToggle()
    {
        base.OnClickStateToggle();

        if (state == WeaponC.State.Poison)
        {
            state = WeaponC.State.Slow;
            stateToggle.Init(false);
        }
        else
        {
            state = WeaponC.State.Poison;
            stateToggle.Init(true);
        }

        weapons[0].SwitchMode(state);
    }
    public override void Init()
    {
        WeaponC weapon = Instantiate<WeaponC>(prefab, transform);
        weapon.gameObject.SetActive(false);

        weapons.Add(weapon);
    }
    public override void Strengthen(int key)
    {
        switch (key)
        {
            case 0: // 획득
            case 1: // 영역 확장
                StrengthenFirst();
                break;
            case 2: // 효과 강화
                weapons[0].PowerUp(++effectLevel);
                break;
            case 99: // 동시 적용
                weapons[0].SwitchMode(WeaponC.State.Both);
                stateToggle.Lock();
                break;
        }
    }
    private void StrengthenFirst()
    {
        if (areaLevel >= WEAPON_COUNT_MAX)
        {
            return;
        }

        AudioManager.Instance.PlaySFX(SoundKey.PlayerGetWeaponC);

        //if (areaLevel++ == 0)
        //{
        //    weapons[0].gameObject.SetActive(true);

        //    stateToggle.Unlock();
        //    stateToggle.Init(state == WeaponC.State.Poison);
        //    weapons[0].Refresh(state);
        //}
        //else
        //{
        //    weapons[0].Expand(areaLevel);
        //}
        if (areaLevel++ == 0)
        {
            weapons[0].gameObject.SetActive(true);

            stateToggle.Unlock();
            stateToggle.Init(state == WeaponC.State.Poison);
        }

        weapons[0].Refresh(state);
        weapons[0].Expand(areaLevel);
    }
}
