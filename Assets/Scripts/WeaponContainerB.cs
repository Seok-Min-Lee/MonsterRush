using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponContainerB : WeaponContainer<WeaponB>
{
    [SerializeField] private StateToggle directionToggle;

    [SerializeField] private float radius = 1f;
    [SerializeField] private float bleedRatio = 0.1f;


    private Queue<WeaponB> bulletPool = new Queue<WeaponB>();
    private float timer = 0f;
    private bool isReverse = false;
    private void Start()
    {
        directionToggle.Init(isReverse);
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
    public void OnClickDirection()
    {
        isReverse = !isReverse;
        directionToggle.Init(isReverse);
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
        base.StrengthenSecond();

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
            float delay = 1f / activeCount;
            
            for (int i = 0; i < activeCount; i++)
            {
                WeaponB bullet = bulletPool.Count > 0 ? 
                                 bulletPool.Dequeue() : 
                                 GameObject.Instantiate<WeaponB>(prefab);

                bullet.Init(
                    container: this, 
                    bleedRatio: bleedRatio, 
                    flipX: isReverse
                );

                float radian = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                Vector2 direction = isReverse ?
                    new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * -1 :
                    new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

                bullet.Shot(
                    position: Player.Instance.transform.position + (Vector3)Random.insideUnitCircle * 0.25f,
                    rotation: transform.rotation,
                    direction: direction
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
