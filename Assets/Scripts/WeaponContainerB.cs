using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponContainerB : WeaponContainer<WeaponB>
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private float bleedRatio = 0.1f;
    [SerializeField] private float detectRaidus = 5f;

    private Queue<WeaponB> bulletPool = new Queue<WeaponB>();
    private float timer = 0f;
    private bool isRandom = false;
    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (timer > 1f)
        {
            if (isRandom)
            {
                LaunchRandom();
            }
            else
            {
                LaunchAuto();
            }

            timer = 0f;
        }

        timer += Time.deltaTime;
    }
    public override void OnClickStateToggle()
    {
        base.OnClickStateToggle();

        isRandom = !isRandom;
        stateToggle.Init(isRandom);
    }
    public override void StrengthenFirst()
    {
        if (activeCount >= WEAPON_COUNT_MAX)
        {
            return;
        }

        if (activeCount++ == 0)
        {
            stateToggle.Unlock();
            stateToggle.Init(isRandom);
        }
    }
    public override void StrengthenSecond()
    {
        bleedRatio += 0.05f;
    }
    public void LaunchForward()
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
                    flipX: isRandom,
                    position: position,
                    rotation: transform.rotation,
                    direction: isRandom ? direction * -1 : direction
                );

                AudioManager.Instance.PlaySFX(SoundKey.WeaponBLaunch);

                yield return new WaitForSeconds(delay);
            }
        }
    }
    private Collider2D[] detectBuffer = new Collider2D[32];
    public void LaunchAuto()
    {
        if (activeCount == 0)
        {
            return;
        }

        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            Vector3 playerPosition = Player.Instance.transform.position;
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.layerMask = LayerMask.GetMask("Enemy");

            int count = activeCount;
            float delay = 1f / count;

            for (int i = 0; i < count; i++)
            {
                Vector3 target;

                // 주변의 적을 탐색하여 가장 가까운 적을 타겟으로 한다.
                int detectCount = Physics2D.OverlapCircle(
                    point: Player.Instance.transform.position,
                    radius: detectRaidus,
                    contactFilter: contactFilter,
                    results: detectBuffer
                );

                // 주변에 적이 없는 경우 랜덤한 방향으로 사출한다.
                if (detectCount < 1)
                {
                    target = playerPosition + (Vector3)Random.insideUnitCircle.normalized;
                }
                else
                {
                    target = detectBuffer.Take(detectCount)
                                         .OrderBy(x => Vector3.SqrMagnitude(x.transform.position - playerPosition))
                                         .First().transform.position;
                }

                Launch(target);

                yield return new WaitForSeconds(delay);
            }
        }
    }
    public void LaunchRandom()
    {
        if (activeCount == 0)
        {
            return;
        }

        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            Vector3 playerPosition = Player.Instance.transform.position;

            int count = activeCount;
            float delay = 1f / count;

            for (int i = 0; i < count; i++)
            {
                Vector3 target = playerPosition + (Vector3)Random.insideUnitCircle.normalized;

                Launch(target);

                yield return new WaitForSeconds(delay);
            }
        }
    }
    public void Launch(Vector3 target)
    {
        WeaponB bullet = bulletPool.Count > 0 ?
                         bulletPool.Dequeue() :
                         GameObject.Instantiate<WeaponB>(prefab);

        //
        Vector3 position = Player.Instance.transform.position + (Vector3)Random.insideUnitCircle * 0.25f;
        Vector3 direction = (target - position).normalized;

        // 방향을 각도로 변환 (라디안 → 도)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //
        bullet.OnShot(
            container: this,
            bleedRatio: bleedRatio,
            flipX: false,
            position: position,
            rotation: Quaternion.Euler(0, 0, angle),
            direction: direction
        );

        AudioManager.Instance.PlaySFX(SoundKey.WeaponBLaunch);
    }
    public void Reload(WeaponB bullet)
    {
        bulletPool.Enqueue(bullet);
    }
    private void OnDrawGizmosSelected()
    {
        if (Player.Instance == null)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Player.Instance.transform.position, detectRaidus);
    }
}
