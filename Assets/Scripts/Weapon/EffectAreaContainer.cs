using UnityEngine;

public class EffectAreaContainer : WeaponOneOnOne<EffectArea>
{
    private EffectArea.State state = EffectArea.State.Poison;
    private int areaLevel = 0;
    private int effectLevel = 0;

    private void Awake()
    {
        base.Init();
    }
    public override void OnClickStateToggle()
    {
        base.OnClickStateToggle();

        if (state == EffectArea.State.Poison)
        {
            state = EffectArea.State.Slow;
            stateToggle.Init(false);
        }
        else
        {
            state = EffectArea.State.Poison;
            stateToggle.Init(true);
        }

        weapon.SwitchMode(state);
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
                weapon.PowerUp(++effectLevel);
                break;
            case 99: // 동시 적용
                state = EffectArea.State.Both;
                weapon.SwitchMode(state);
                stateToggle.Lock();
                break;
        }
    }
    private void StrengthenFirst()
    {
        if (areaLevel >= LEVEL_MAX)
        {
            return;
        }

        AudioManager.Instance.PlaySFX(SoundKey.PlayerGetWeaponC);

        if (areaLevel++ == 0)
        {
            weapon.gameObject.SetActive(true);

            stateToggle.Unlock();
            stateToggle.Init(state == EffectArea.State.Poison);
        }

        weapon.Refresh(state);
        weapon.Expand(areaLevel);
    }
}
