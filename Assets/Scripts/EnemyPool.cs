using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private Enemy prefab;

    public int speedLevel;
    public int hpLevel;
    public int powerLevel;

    public float spawnInterval;
    public int poolSizeMax;
    private float spawnDistanceMin;
    private float spawnDistanceMax;
    private int poolSizeMin;

    private Transform container;

    private int count;

    private Queue<Enemy> pool = new Queue<Enemy>();

    private float timer = 0f;
    private void Update()
    {
        if (StaticValues.isTutorial)
        {
            return;
        }

        if (timer > spawnInterval)
        {
            Spawn();
            timer = 0f;
        }

        timer += Time.deltaTime;
    }
    public void Charge(Enemy enemy)
    {
        pool.Enqueue(enemy);
        count--;
    }

    private void Spawn()
    {
        if (count < poolSizeMax)
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

            count++;
        }
    }
    public void Init(
        Transform container, 
        float spawnInterval, 
        float spawnDistanceMin, 
        float spawnDistanceMax, 
        int poolSizeMin, 
        int poolSizeMax
    )
    {
        this.container = container;
        this.spawnInterval = spawnInterval;
        this.spawnDistanceMin = spawnDistanceMin;
        this.spawnDistanceMax = spawnDistanceMax;
        this.poolSizeMin = poolSizeMin;
        this.poolSizeMax = poolSizeMax;

        for (int i = 0; i < poolSizeMin; i++)
        {
            Enemy enemy = Instantiate(prefab, container).GetComponent<Enemy>();
            enemy.gameObject.SetActive(false);
            pool.Enqueue(enemy);
        }
    }

}
