using UnityEngine;

public class EnemyContainer : MonoBehaviour
{
    public static EnemyContainer Instance { get; private set; }

    [SerializeField] private EnemyPool[] pools;
    [SerializeField] private int stage = 0;

    [SerializeField] private float spawnInterval;
    [SerializeField] private float spawnDistanceMin;
    [SerializeField] private float spawnDistanceMax;
    [SerializeField] private int poolSizeMin;
    [SerializeField] private int poolSizeMax;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < pools.Length; i++)
        {
            pools[i].Init(
                container: transform,
                spawnInterval: spawnInterval,
                spawnDistanceMin: spawnDistanceMin,
                spawnDistanceMax: spawnDistanceMax,
                poolSizeMin: poolSizeMin,
                poolSizeMax: poolSizeMax
            );
        }
        pools[0].gameObject.SetActive(true);
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
            pools[stage++].gameObject.SetActive(false);
            pools[stage].gameObject.SetActive(true);

            spawnInterval *= 0.9f;
            poolSizeMax = (int)(poolSizeMax * 1.1f);
            for (int i = stage; i < pools.Length; i++)
            {
                pools[i].spawnInterval *= spawnInterval;
                pools[i].poolSizeMax = poolSizeMax;
            }
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
