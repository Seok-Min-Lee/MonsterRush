using UnityEngine;

public class KnifeLauncher : WeaponLauncher<Knife>
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private float detectRaidus = 5f;

    private bool isPenetrate = false;
    private bool isRandomTarget = false;
    private int bleedLevel = 0;
    private int frequencyLevel = 0;

    private float timer = 0f;
    private float interval = 1f;

    private void Update()
    {
        if (Time.timeScale == 0f || StaticValues.isWait || frequencyLevel == 0)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer > interval)
        {
            // 가장 가까운 적을 찾기
            Enemy nearest = null;
            float sqrMagnitude = float.MaxValue;

            if (!isRandomTarget)
            {
                foreach (Enemy enemy in EnemyContainer.Instance.GetActiveEnemyAll())
                {
                    if (enemy.toPlayer.sqrMagnitude < sqrMagnitude)
                    {
                        nearest = enemy;
                        sqrMagnitude = enemy.toPlayer.sqrMagnitude;
                    }
                }
            }

            Vector3 target = sqrMagnitude < 25 ? 
                     nearest.transform.position : 
                     Player.Instance.transform.position + (Vector3)Random.insideUnitCircle.normalized;

            Launch(target);
            timer = 0f;
        }
    }
    public override void OnClickStateToggle()
    {
        base.OnClickStateToggle();

        isRandomTarget = !isRandomTarget;
        stateToggle.SetState(isRandomTarget);
    }
    public override void Strengthen(int key)
    {
        switch (key)
        {
            case 0: // 획득
            case 1: // 개수 증가
                StrengthenFirst();
                break;
            case 2: // 확률 증가
                bleedLevel++;
                break;
            case 99: // 관통 활성화
                isPenetrate = true;
                break;
        }
    }
    private void StrengthenFirst()
    {
        if (frequencyLevel >= LEVEL_MAX)
        {
            return;
        }

        if (frequencyLevel++ == 0)
        {
            stateToggle.Unlock();
            stateToggle.SetState(isRandomTarget);
        }

        interval = 1f / frequencyLevel;
    }
    public override void Launch(Vector3 target)
    {
        Knife bullet = bulletPool.Count > 0 ?
                         bulletPool.Dequeue() :
                         GameObject.Instantiate<Knife>(prefab);

        //
        Vector3 position = Player.Instance.transform.position + (Vector3)Random.insideUnitCircle * 0.25f;
        Vector3 direction = (target - position).normalized;

        // 방향을 각도로 변환 (라디안 → 도)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //
        bullet.OnShot(
            container: this,
            bleedLevel: bleedLevel,
            isPenetrate: isPenetrate,
            position: position,
            rotation: Quaternion.Euler(0, 0, angle),
            direction: direction
        );
        actives.Add(bullet);

        AudioManager.Instance.PlaySFX(SoundKey.WeaponBLaunch);
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
