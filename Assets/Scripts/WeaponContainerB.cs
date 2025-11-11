using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponContainerB : WeaponContainer<WeaponB>
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private float bleedRatio = 0.1f;


    private Queue<WeaponB> bulletPool = new Queue<WeaponB>();
    private float timer = 0f;
    private bool isReverse = false;
    private void Start()
    {
        stateToggle.Init(isReverse);
    }
    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (timer > 1f)
        {
            Launch();

            timer = 0f;
        }

        timer += Time.deltaTime;
    }
    public override void OnClickStateToggle()
    {
        base.OnClickStateToggle();

        isReverse = !isReverse;
        stateToggle.Init(isReverse);
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
        bleedRatio += 0.05f;
    }
    public void Launch()
    {
        if (activeCount == 0)
        {
            return;
        }

        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            int count = activeCount;
            float delay = 1f / count;
            
            for (int i = 0; i < count; i++)
            {
                WeaponB bullet = bulletPool.Count > 0 ? 
                                 bulletPool.Dequeue() : 
                                 GameObject.Instantiate<WeaponB>(prefab);

                //
                float radian = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
                Vector3 position = Player.Instance.transform.position + (Vector3)Random.insideUnitCircle * 0.25f;

                //
                bullet.OnShot(
                    container: this,
                    bleedRatio: bleedRatio,
                    flipX: isReverse,
                    position: position,
                    rotation: transform.rotation,
                    direction: isReverse ? direction * -1 : direction
                );

                AudioManager.Instance.PlaySFX(SoundKey.WeaponBLaunch);

                yield return new WaitForSeconds(delay);
            }
        }
    }
    public void Reload(WeaponB bullet)
    {
        bulletPool.Enqueue(bullet);
    }
}
