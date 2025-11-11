using System.Collections.Generic;
using UnityEngine;

public class WeaponContainer<T> : MonoBehaviour where T : Weapon
{
    protected int WEAPON_COUNT_MAX = 8;

    [SerializeField] protected T prefab;
    [SerializeField] protected StateToggle stateToggle;

    protected List<T> weapons = new List<T>();
    protected int activeCount = 0;
    public virtual void OnClickStateToggle()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);
    }
    public virtual void Init()
    {
        for (int i = 0; i < WEAPON_COUNT_MAX; i++)
        {
            T weapon = Instantiate<T>(prefab, transform);
            weapon.gameObject.SetActive(false);

            weapons.Add(weapon);
        }
    }
    public virtual void StrengthenFirst()
    {
        if (activeCount >= WEAPON_COUNT_MAX)
        {
            return;
        }

        weapons[activeCount++].gameObject.SetActive(true);
    }
    public virtual void StrengthenSecond()
    {
        return;
    }
    public virtual void Rotate(float x, float y, float z)
    {
        transform.rotation = Quaternion.Euler(x, y, z);
    }
}
