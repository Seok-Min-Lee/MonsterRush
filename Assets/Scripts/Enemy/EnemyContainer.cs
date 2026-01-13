using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour
{
    public static EnemyContainer Instance { get; private set; }

    public EnemyPool currentPool => pools[Mathf.Min(stage, pools.Length)];
    
    [SerializeField] private EnemyPool[] pools;
    [SerializeField] private EnemyPool epicPool;
    [SerializeField] private int stage = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        for (int i = 1; i < pools.Length; i++)
        {
            pools[i].gameObject.SetActive(false);
            pools[i].SetSpawnState(false);
        }
        pools[0].gameObject.SetActive(true);
        pools[0].SetSpawnState(true);

        epicPool.gameObject.SetActive(false);
        epicPool.SetSpawnState(false);
    }
    public List<Enemy> GetActiveEnemyAll()
    {
        List<Enemy> enemies = new List<Enemy>();

        for (int i = 0; i < pools.Length; i++)
        {
            enemies.AddRange(pools[i].actives);
        }
        enemies.AddRange(epicPool.actives);

        return enemies;
    }
    public void UpdateByLevel(int level)
    {
        // 레벨에 따른 등급 및 강화 처리
        int remain = level % 10;
        switch (remain)
        {
            // 10레벨마다 등급 상승
            // 최상위 등급 도달 후에는 스폰 레벨 상승
            case 0:
                if (level < StaticValues.CHECKPOINT_LEVEL)
                {
                    stage = level / 10;

                    for (int i = 0; i < pools.Length; i++)
                    {
                        pools[i].SetSpawnState(i == stage);
                    }

                    pools[stage].gameObject.SetActive(true);
                }
                else
                {
                    pools[stage].SetSpawnLevel((level - StaticValues.CHECKPOINT_LEVEL) / 10);
                }
                break;
            // 적 공격력 증가 3회
            case 1:
            case 4:
            case 7:
                pools[stage].SetPowerLevel(remain / 3 + 1);
                break;
            // 적 이동속도 증가 3회
            case 2:
            case 5:
            case 8:
                pools[stage].SetSpeedLevel(remain / 3 + 1);
                break;
            // 적 체력 증가 3회
            case 3:
            case 6:
            case 9:
                pools[stage].SetHpLevel(remain / 3);
                break;
            default:
                break;
        }

        // 에픽 등급 처리
        if (level == StaticValues.CHECKPOINT_LEVEL)
        {
            epicPool.SetSpawnState(true);
            epicPool.gameObject.SetActive(true);
        }
        else if (level > StaticValues.CHECKPOINT_LEVEL)
        {
            int epicLevel = level - StaticValues.CHECKPOINT_LEVEL;
            epicPool.SetSpawnLevel(epicLevel);
            epicPool.SetHpLevel(epicLevel);
            epicPool.SetPowerLevel(epicLevel);
            epicPool.SetSpeedLevel(epicLevel);
        }
    }
}
