using System.Collections.Generic;
using UnityEngine;

public class WeaponLauncher<T> : WeaponControllerBase where T : Weapon
{
    [SerializeField] protected T prefab;

    protected Queue<T> bulletPool = new Queue<T>();
    protected List<T> actives = new List<T>();
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
        return;
    }
    public override void Strengthen(int key)
    {
        return;
    }
    public virtual void Launch(Vector3 target)
    {
        return;
    }
    public virtual void Charge(T bullet)
    {
        actives.Remove(bullet);
        bulletPool.Enqueue(bullet);
    }
}
