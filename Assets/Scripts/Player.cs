using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] private Transform[] weaponContainers;
    [SerializeField] private CircleCollider2D magnetCollider;

    [SerializeField] private float speed = 0.025f;
    [SerializeField] private int strength = 1;

    [Header("UI")]
    [SerializeField] private VariableJoystick joystick;
    [SerializeField] private GameObject canvasGO;
    [SerializeField] private Image hpGuage;
    [SerializeField] private Image expGuage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private TextMeshProUGUI magnetText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI[] weaponTexts;

    public int Strength => strength;
    public Vector3 moveVec { get; private set; }

    public Animator animator { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }

    private int exp = 0;
    private int expMax = 100;

    private int hp = 100;
    private int hpMax = 100;

    public int killCount { get; private set; } = 0;
    private int level = 1;
    private int lifeCount = 0;
    private int magnetLevel = 1;
    private int speedLevel = 1;
    private int strengthLevel = 1;
    private int[] weaponLevels;

    private void Awake()
    {
        Instance = this;

        weaponLevels = new int[weaponTexts.Length];

        GameObject character = GameObject.Instantiate(Resources.Load<GameObject>("Players/Character_" + StaticValues.playerCharacterNum), transform);
        animator = character.GetComponent<Animator>();
        spriteRenderer = character.GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        canvasGO.SetActive(false);
        hpGuage.fillAmount = 1f;

        expGuage.fillAmount = 0f;
        levelText.text = level.ToString();
        killText.text = killCount.ToString();
        magnetText.text = magnetLevel.ToString();
        speedText.text = speedLevel.ToString();
        strengthText.text = strengthLevel.ToString();
        lifeText.text = lifeCount.ToString();
        for (int i = 0; i < weaponTexts.Length; i++)
        {
            weaponTexts[i].text = weaponLevels[i].ToString();
        }
    }
    private void FixedUpdate()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
#if !UNITY_EDITOR
        moveVec = new Vector3(joystick.Horizontal, joystick.Vertical, 0f) * speed;
        transform.position += moveVec;
        spriteRenderer.flipX = moveVec.x < 0;

        float angle = Mathf.Atan2(joystick.Vertical, joystick.Horizontal) * Mathf.Rad2Deg;
        weaponContainers[1].rotation = Quaternion.Euler(0, 0, angle);
#endif
        transform.position += moveVec;
    }
    public void KillEnemy()
    {
        killCount++;
        killText.text = killCount.ToString();
    }
    public void GainExp(int value)
    {
        exp += value;

        if (exp > expMax)
        {
            level++;
            exp -= expMax;
            expMax = (int)(expMax * 1.1f);

            levelText.text = level.ToString();

            // 입력 초기화
            var pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
                position = Input.mousePosition
            };
            ExecuteEvents.Execute<IPointerUpHandler>(joystick.gameObject, pointerData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute<IEndDragHandler>(joystick.gameObject, pointerData, ExecuteEvents.endDragHandler);

            //
            GameCtrl.Instance.OnLevelUp();
        }

        expGuage.fillAmount = exp / (float)expMax;
        Debug.Log(expGuage.fillAmount);
    }
    public void GainReward(RewardInfo rewardInfo)
    {
        switch (rewardInfo.id)
        {
            case 0:
                weaponContainers[rewardInfo.id].GetComponent<WeaponContainerA>().Add();
                weaponLevels[rewardInfo.id]++;
                weaponTexts[rewardInfo.id].text = weaponLevels[rewardInfo.id].ToString();
                break;
            case 1:
                weaponContainers[rewardInfo.id].GetComponent<WeaponContainerB>().Add();
                weaponLevels[rewardInfo.id]++;
                weaponTexts[rewardInfo.id].text = weaponLevels[rewardInfo.id].ToString();
                break;
            case 2:
                weaponContainers[rewardInfo.id].GetComponent<WeaponContainerC>().Add();
                weaponLevels[rewardInfo.id]++;
                weaponTexts[rewardInfo.id].text = weaponLevels[rewardInfo.id].ToString();
                break;
            case 3:
                weaponContainers[rewardInfo.id].GetComponent<WeaponContainerD>().Add();
                weaponLevels[rewardInfo.id]++;
                weaponTexts[rewardInfo.id].text = weaponLevels[rewardInfo.id].ToString();
                break;
            case 4:
                magnetCollider.radius += 0.1f;
                magnetLevel++;
                magnetText.text = magnetLevel.ToString();
                break;
            case 5:
                speed *= 1.05f;
                speedLevel++;
                speedText.text = speedLevel.ToString();
                break;
            case 6:
                strength += 1;
                strengthLevel++;
                strengthText.text = strengthLevel.ToString();
                break;
            case 7:
                lifeCount += 1;
                lifeText.text = lifeCount.ToString();
                break;
        }
    }
    public virtual void OnDamage(int damage)
    {
        hp -= damage;

        if (hp > 0)
        {
            canvasGO.SetActive(true);
            hpGuage.fillAmount = (float)hp / (float)hpMax;

            animator.SetTrigger("doHit");
        }
        else
        {
            GameCtrl.Instance.OnGameEnd();
        }
    }
    private void OnMove(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();

        if (v.sqrMagnitude > 0.001f)
        {
            moveVec = new Vector3(v.x, v.y, 0f) * speed;
            animator.SetBool("isMove", true);

            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            weaponContainers[1].rotation = Quaternion.Euler(0, 0, angle);

            spriteRenderer.flipX = moveVec.x < 0;
        }
        else
        {
            moveVec = Vector3.zero;
            animator.SetBool("isMove", false);
        }
    }
}
