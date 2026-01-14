using System.Collections.Generic;
using UnityEngine;

public class EnemyContainerGroup : MonoBehaviour
{
    public static EnemyContainerGroup Instance { get; private set; }

    public EnemyContainer currentContainer => containers[Mathf.Min(stage, containers.Length)];
    
    [SerializeField] private EnemyContainer[] containers;
    [SerializeField] private EnemyContainer epicContainer;
    [SerializeField] private int stage = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        for (int i = 1; i < containers.Length; i++)
        {
            containers[i].gameObject.SetActive(false);
            containers[i].SetSpawnState(false);
        }
        containers[0].gameObject.SetActive(true);
        containers[0].SetSpawnState(true);

        epicContainer.gameObject.SetActive(false);
        epicContainer.SetSpawnState(false);
    }
    public List<Enemy> GetActiveEnemyAll()
    {
        List<Enemy> enemies = new List<Enemy>();

        for (int i = 0; i < containers.Length; i++)
        {
            enemies.AddRange(containers[i].actives);
        }
        enemies.AddRange(epicContainer.actives);

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

                    for (int i = 0; i < containers.Length; i++)
                    {
                        containers[i].SetSpawnState(i == stage);
                    }

                    containers[stage].gameObject.SetActive(true);
                }
                else
                {
                    containers[stage].SetSpawnLevel((level - StaticValues.CHECKPOINT_LEVEL) / 10);
                }
                break;
            // 적 공격력 증가 3회
            case 1:
            case 4:
            case 7:
                containers[stage].SetPowerLevel(remain / 3 + 1);
                break;
            // 적 이동속도 증가 3회
            case 2:
            case 5:
            case 8:
                containers[stage].SetSpeedLevel(remain / 3 + 1);
                break;
            // 적 체력 증가 3회
            case 3:
            case 6:
            case 9:
                containers[stage].SetHpLevel(remain / 3);
                break;
            default:
                break;
        }

        // 에픽 등급 처리
        if (level == StaticValues.CHECKPOINT_LEVEL)
        {
            epicContainer.SetSpawnState(true);
            epicContainer.gameObject.SetActive(true);
        }
        else if (level > StaticValues.CHECKPOINT_LEVEL)
        {
            int epicLevel = level - StaticValues.CHECKPOINT_LEVEL;
            epicContainer.SetSpawnLevel(epicLevel);
            epicContainer.SetHpLevel(epicLevel);
            epicContainer.SetPowerLevel(epicLevel);
            epicContainer.SetSpeedLevel(epicLevel);
        }
    }
}
