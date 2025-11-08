using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponContainerD : WeaponContainer<WeaponD>
{
    [SerializeField] private int knockbackPower = 1;

    private Queue<WeaponD> bulletPool = new Queue<WeaponD>();
    private float timer = 0f;
    private bool isSequence = false;
    private void Start()
    {
        stateToggle.Init(isSequence);
    }
    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (timer > 2f)
        {
            if (isSequence)
            {
                LaunchSequence();
            }
            else
            {
                LaunchOnce();
            }

            timer = 0f;
        }

        timer += Time.deltaTime;
    }
    public override void OnClickStateToggle()
    {
        base.OnClickStateToggle();

        isSequence = !isSequence;
        stateToggle.Init(isSequence);
        timer = 0f;
    }
    public override void StrengthenFirst()
    {
        if (activeCount >= WEAPON_COUNT_MAX)
        {
            return;
        }
        activeCount++;
    }
    public override void StrengthenSecond()
    {
        knockbackPower++;
    }

    public void LaunchOnce()
    {
        if (activeCount <= 0)
        {
            return;
        }

        for (int i = 0; i < activeCount; i++)
        {
            Launch();
        }
        AudioManager.Instance.PlaySFX(SoundKey.WeaponDLaunch);
    }
    public void LaunchSequence()
    {
        if (activeCount <= 0)
        {
            return;
        }

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

        bullet.Init(this, knockbackPower);

        float radian = Random.Range(60, 120) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

        bullet.Shot(
            position: transform.position,
            rotation: transform.rotation,
            direction: direction
        );
    }
    public void Reload(WeaponD bullet)
    {
        bulletPool.Enqueue(bullet);
    }
}
