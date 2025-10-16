using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponContainerB : WeaponContainer<WeaponB>
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private Bullet bulletPrefab;

    private Queue<Bullet> bulletPool = new Queue<Bullet>();
    private float timer = 0f;
    private void Start()
    {
        base.Init();
    }
    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Add();
        }

        if (timer > 1f)
        {
            Launch();

            timer = 0f;
        }

        timer += Time.deltaTime;
    }
    public override void Add()
    {
        base.Add();

        List<WeaponB> actives = new List<WeaponB>();
        actives.AddRange(weapons.Where(x => x.gameObject.activeSelf));

        int activeCount = actives.Count;
        for (int i = 0; i < activeCount; i++)
        {
            float angle = i * Mathf.PI * 2f / activeCount + 15; // 0 ~ 360도 균등 분할 (라디안)
            Vector3 pos = Player.Instance.transform.position + new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * radius;

            actives[i].transform.position = pos;
        }
    }

    public void Launch()
    {
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            List<Weapon> actives = new List<Weapon>();
            for (int i = 0; i < activeCount; i++)
            {
                actives.Add(weapons[i]);
            }

            float delay = 1f / actives.Count;
            for (int i = 0; i < actives.Count; i++)
            {
                Bullet bullet;
                if (bulletPool.Count > 0)
                {
                    bullet = bulletPool.Dequeue();
                }
                else
                {
                    bullet = GameObject.Instantiate<Bullet>(bulletPrefab);
                    bullet.Init(this);
                }

                float radian = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

                bullet.Shot(
                    position: actives[i].transform.position,
                    rotation: transform.rotation,
                    direction: direction
                );

                yield return new WaitForSeconds(delay);
            }
        }
    }
    public void Reload(Bullet bullet)
    {
        bulletPool.Enqueue(bullet);
    }
}
