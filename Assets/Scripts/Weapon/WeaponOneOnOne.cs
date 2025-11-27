using UnityEngine;

public class WeaponOneOnOne<T> : WeaponControllerBase where T : Weapon
{
    [SerializeField] protected T prefab;

    protected T weapon;
    public override void OnClickStateToggle()
    {
        if (stateToggle == null)
        {
            return;
        }

        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
    }
    public override void Init()
    {
        weapon = Instantiate<T>(prefab, transform);
        weapon.gameObject.SetActive(false);
    }
    public override void Strengthen(int key)
    {
        return;
    }
}
