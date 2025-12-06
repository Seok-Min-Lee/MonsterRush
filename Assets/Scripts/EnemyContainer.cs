using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour
{
    public static EnemyContainer Instance { get; private set; }

    public EnemyPool currentPool => pools[stage];
    
    [SerializeField] private EnemyPool[] pools;
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
            pools[i].isSpawn = false;
        }
        pools[0].gameObject.SetActive(true);
        pools[0].isSpawn = true;
    }
    public List<Enemy> GetActiveEnemyAll()
    {
        List<Enemy> enemies = new List<Enemy>();

        for (int i = 0; i < pools.Length; i++)
        {
            enemies.AddRange(pools[i].actives);
        }

        return enemies;
    }
    public void OnLevelUp()
    {
        // 난이도 상승 구조
        // 10 레벨동안 등급 상승 1회, 능력치 강화 9회
        int remain = Player.Instance.Level % 10;

        switch (remain)
        {
            // 적 등급 상승
            case 0:
                GradeUp();
                break;

            // 적 공격력 증가 3회
            case 1:
            case 4:
            case 7:
                PowerUp();
                break;

            // 적 이동속도 증가 3회
            case 2:
            case 5:
            case 8:
                SpeedUp();
                break;

            // 적 체력 증가 3회
            case 3:
            case 6:
            case 9:
                HpUp();
                break;
            default:
                break;
        }
    }
    private void GradeUp()
    {
        if (stage < pools.Length - 1)
        {
            for (int i = 0; i < stage; i++)
            {
                if (pools[i].gameObject.activeSelf && pools[i].actives.Count == 0)
                {
                    pools[i].gameObject.SetActive(false);
                }
            }

            pools[stage++].isSpawn = false;
            pools[stage].gameObject.SetActive(true);
            pools[stage].isSpawn = true;
        }
        else
        {
            pools[stage].GradeUp();
        }
    }
    private void SpeedUp()
    {
        pools[stage].speedLevel++;
    }
    private void HpUp()
    {
        pools[stage].hpLevel++;
    }
    private void PowerUp()
    {
        pools[stage].powerLevel++;
    }
}
