using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BombLauncher : WeaponLauncher<Bomb>
{
    public enum State { None, Fire, Reloading }

    [SerializeField] private float reloadDelay;

    private State state = State.None;
    private bool isScaleUp = false;
    private bool isUpper = true;
    private int launchLevel = 0;
    private int knockbackLevel = 0;

    private float timer = 0f;
    private int bulletCount = 0;

    private void Update()
    {
        if (Time.timeScale == 0f || StaticValues.isWait || launchLevel == 0)
        {
            return;
        }

        timer += Time.deltaTime;

        if (state == State.Fire)
        {
            if (bulletCount < launchLevel && timer > 0.25f)
            {
                Launch(Vector3.zero);

                timer = 0f;

                if (++bulletCount == launchLevel)
                {
                    bulletCount = 0;
                    state = State.Reloading;
                }
            }
        }
        else if (state == State.Reloading)
        {
            if (timer > reloadDelay)
            {
                state = State.Fire;
            }
        }
    }
    public override void OnClickStateToggle()
    {
        base.OnClickStateToggle();

        isUpper = !isUpper;
        stateToggle.Init(isUpper);
        timer = 0f;
    }
    public override void Strengthen(int key)
    {
        switch (key)
        {
            case 0: // È¹µæ
            case 1: // °³¼ö Áõ°¡
                StrengthenFirst();
                break;
            case 2: // ³Ë¹é °­È­
                knockbackLevel++;
                break;
            case 99: // Æø¹ß È®Àå
                isScaleUp = true;
                break;
        }
    }
    public override void Launch(Vector3 target)
    {
        Bomb bullet = bulletPool.Count > 0 ?
                      bulletPool.Dequeue() :
                      GameObject.Instantiate<Bomb>(prefab, transform);

        //
        float radian, mass;
        Vector3 position;
        if (isUpper)
        {
            position = transform.position + Vector3.up * 0.25f;
            mass = Random.Range(8f, 16f);
            radian = Random.Range(60, 120) * Mathf.Deg2Rad;
        }
        else
        {
            position = transform.position + Vector3.down * 0.25f;
            mass = Random.Range(6f, 8f);
            radian = Random.Range(150, 390) * Mathf.Deg2Rad;
        }

        Vector2 force = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * mass;
        float torque = Random.Range(-10f, 10f);

        //
        bullet.OnShot(
            container: this,
            isScaleUp: isScaleUp,
            knockbackLevel: knockbackLevel,
            position: position,
            force: force,
            torque: torque
        );

        actives.Add(bullet);

        AudioManager.Instance.PlaySFX(SoundKey.WeaponDLaunch);
    }
    private void StrengthenFirst()
    {
        if (launchLevel >= LEVEL_MAX)
        {
            return;
        }

        if (launchLevel == 0)
        {
            stateToggle.Unlock();
            stateToggle.Init(isUpper);

            state = State.Fire;
            bulletCount = 0;
        }

        launchLevel++;
    }

}
