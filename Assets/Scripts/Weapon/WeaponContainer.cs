using System.Collections.Generic;
using UnityEngine;
public class WeaponContainer<T> : WeaponControllerBase where T : Weapon
{
    [SerializeField] protected T prefab;

    protected List<T> weapons = new List<T>();
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
        for (int i = 0; i < LEVEL_MAX; i++)
        {
            T weapon = Instantiate<T>(prefab, transform);
            weapon.gameObject.SetActive(false);

            weapons.Add(weapon);
        }
    }
    public override void Strengthen(int key)
    {
        return;
    }
}
