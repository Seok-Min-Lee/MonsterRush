using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponContainerD : WeaponContainer<WeaponD>
{
    [SerializeField] private int knockbackLevel = 1;

    private Queue<WeaponD> bulletPool = new Queue<WeaponD>();
    private float timer = 0f;
    private bool isUpper = false;
    private float explosionScale = 1f;

    private void Update()
    {
        if (Time.timeScale == 0f || activeCount == 0)
        {
            return;
        }

        if (timer > 2.5f)
        {
            LaunchSequence();

            timer = 0f;
        }

        timer += Time.deltaTime;
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
            case 0: // 개수 증가
                StrengthenFirst();
                break;
            case 1: // 넉백 강화
                knockbackLevel++;
                break;
            case 99: // 폭발 확장
                explosionScale = 1.75f;
                break;
        }
    }
    private void StrengthenFirst()
    {
        if (activeCount >= WEAPON_COUNT_MAX)
        {
            return;
        }

        if (activeCount++ == 0)
        {
            stateToggle.Unlock();
            stateToggle.Init(isUpper);
        }
    }

    public void LaunchSequence()
    {
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            for (int i = 0; i < activeCount; i++)
            {
                Launch();
                AudioManager.Instance.PlaySFX(SoundKey.WeaponDLaunch);

                yield return new WaitForSeconds(0.25f);
            }
        }
    }
    private void Launch()
    {
        WeaponD bullet = bulletPool.Count > 0 ?
                         bulletPool.Dequeue() :
                         GameObject.Instantiate<WeaponD>(prefab, transform);
        
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
            knockbackLevel: knockbackLevel,
            explosionScale: explosionScale,
            position: position,
            force: force,
            torque: torque
        );
    }
    public void Reload(WeaponD bullet)
    {
        bulletPool.Enqueue(bullet);
    }
}
