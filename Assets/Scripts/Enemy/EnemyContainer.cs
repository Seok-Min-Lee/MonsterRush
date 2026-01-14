using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour
{
    [SerializeField] private Enemy prefab;

    [Header("Init Values")]
    [SerializeField] private float spawnInterval;
    [SerializeField] private float spawnDistanceMin;
    [SerializeField] private float spawnDistanceMax;
    [SerializeField] private int countMax;

    public bool isSpawn { get; private set; }
    public int speedLevel { get; private set; }
    public int hpLevel { get; private set; }
    public int powerLevel { get; private set; }
    public int SpawnLevel { get; private set; }

    private float spawnDelay;

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

            if (timer > spawnDelay)
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
    public void SetSpawnState(bool value)
    {
        isSpawn = value;
        spawnDelay = spawnInterval;
    }
    public void SetSpawnLevel(int value)
    {
        SpawnLevel = value;
        spawnDelay = spawnInterval * Mathf.Pow(0.9f, SpawnLevel);
    }
    public void SetSpeedLevel(int value)
    {
        speedLevel = value;
    }
    public void SetHpLevel(int value)
    {
        hpLevel = value;
    }
    public void SetPowerLevel(int value)
    {
        powerLevel = value;
    }
    public void Charge(Enemy enemy)
    {
        pool.Enqueue(enemy);
        actives.Remove(enemy);

        if (!isSpawn && actives.Count == 0)
        {
            gameObject.SetActive(false);
        }
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
                enemy = Instantiate<Enemy>(prefab, transform.parent);
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
    private void OnDrawGizmosSelected()
    {
        if (Player.Instance == null)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Player.Instance.transform.position, spawnDistanceMax);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Player.Instance.transform.position, spawnDistanceMin);
    }
}
