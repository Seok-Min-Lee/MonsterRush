using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private Enemy prefab;

    public bool isSpawn = false;
    public int speedLevel;
    public int hpLevel;
    public int powerLevel;

    public float spawnInterval;
    public int countMax;
    private float spawnDistanceMin;
    private float spawnDistanceMax;
    private int poolSizeMin;

    private Transform container;

    public List<Enemy> actives { get; private set; } = new List<Enemy>();
    private Queue<Enemy> pool = new Queue<Enemy>();

    private float timer = 0f;
    private void Update()
    {
        if (StaticValues.isWait)
        {
            return;
        }

        if (isSpawn)
        {
            timer += Time.deltaTime;

            if (timer > spawnInterval)
            {
                Spawn();
                timer = 0f;
            }
        }

        if (actives.Count > 0)
        {
            Enemy[] snapshot = actives.ToArray();

            for (int i = 0; i < snapshot.Length; i++)
            {
                actives[i].UpdateTick(Time.deltaTime);
            }
        }
    }
    public void Charge(Enemy enemy)
    {
        pool.Enqueue(enemy);
        actives.Remove(enemy);
    }

    private void Spawn()
    {
        if (actives.Count < countMax)
        {
            // 
            Vector2 direction = Random.insideUnitCircle.normalized;
            float distance = Random.Range(spawnDistanceMin, spawnDistanceMax);

            Vector2 position = (Vector2)Player.Instance.transform.position + direction * distance;

            //
            Enemy enemy;
            if (pool.Count > 0)
            {
                enemy = pool.Dequeue();
                enemy.gameObject.SetActive(true);
            }
            else
            {
                enemy = Instantiate<Enemy>(prefab, container);
            }

            enemy.Spawn(
                pool: this,
                hpLevel: hpLevel,
                speedLevel: speedLevel,
                powerLevel: powerLevel,
                position: position, 
                rotation: Quaternion.identity
            );

            actives.Add(enemy);
        }
    }
    public void Init(
        Transform container, 
        float spawnInterval, 
        float spawnDistanceMin, 
        float spawnDistanceMax, 
        int countMax
    )
    {
        this.container = container;
        this.spawnInterval = spawnInterval;
        this.spawnDistanceMin = spawnDistanceMin;
        this.spawnDistanceMax = spawnDistanceMax;
        this.countMax = countMax;

        for (int i = 0; i < poolSizeMin; i++)
        {
            Enemy enemy = Instantiate<Enemy>(prefab, container);
            enemy.gameObject.SetActive(false);
            pool.Enqueue(enemy);
        }
    }

}
