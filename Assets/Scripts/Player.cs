using System.Collections.Generic;
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

    [Header("UI")]
    [SerializeField] private VariableJoystick joystick;
    [SerializeField] private GameObject canvasGO;
    [SerializeField] private Image hpGuage;
    [SerializeField] private Image expGuage;
    [SerializeField] private StatSlot[] statSlots;
    private Dictionary<PlayerStat, StatSlot> statDictionary;

    public int Level => statDictionary[PlayerStat.Level].value;
    public int Strength => statDictionary[PlayerStat.Strength].value;
    public int killCount => statDictionary[PlayerStat.Kill].value;

    public Vector3 moveVec { get; private set; }
    public Animator animator { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }

    private void Awake()
    {
        Instance = this;

        // 캐릭터 선택
        GameObject character = GameObject.Instantiate(Resources.Load<GameObject>("Players/Character_" + StaticValues.playerCharacterNum), transform);
        animator = character.GetComponent<Animator>();
        spriteRenderer = character.GetComponent<SpriteRenderer>();

    }
    private void Start()
    {
        canvasGO.SetActive(false);

        // 스탯 초기화
        statDictionary = new Dictionary<PlayerStat, StatSlot>();
        foreach (StatSlot slot in statSlots)
        {
            statDictionary.Add(slot.type, slot);
        }
        statDictionary[PlayerStat.Level].Init(1);
        statDictionary[PlayerStat.Kill].Init(0);
        statDictionary[PlayerStat.Magnet].Init(1);
        statDictionary[PlayerStat.Speed].Init(1);
        statDictionary[PlayerStat.Strength].Init(1);
        statDictionary[PlayerStat.Life].Init(0);
        statDictionary[PlayerStat.WeaponA].Init(StaticValues.playerCharacterNum == 0 ? 1 : 0);
        statDictionary[PlayerStat.WeaponB].Init(StaticValues.playerCharacterNum == 1 ? 1 : 0);
        statDictionary[PlayerStat.WeaponC].Init(StaticValues.playerCharacterNum == 2 ? 1 : 0);
        statDictionary[PlayerStat.WeaponD].Init(StaticValues.playerCharacterNum == 3 ? 1 : 0);
        statDictionary[PlayerStat.Hp].Init(100);
        statDictionary[PlayerStat.HpMax].Init(100);
        statDictionary[PlayerStat.Exp].Init(0);
        statDictionary[PlayerStat.ExpMax].Init(100);

        hpGuage.fillAmount = 1f;
        expGuage.fillAmount = 0f;
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
        statDictionary[PlayerStat.Kill].Increase();
    }
    public void GainExp(int value)
    {
        int exp = statDictionary[PlayerStat.Exp].value;
        int expMax = statDictionary[PlayerStat.ExpMax].value;

        exp += value;

        if (exp > expMax)
        {
            exp -= expMax;
            expMax = (int)(expMax * 1.1f);

            statDictionary[PlayerStat.Level].Increase();
            statDictionary[PlayerStat.ExpMax].Init(expMax);

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
            EnemyContainer.Instance.OnLevelUp();
        }

        statDictionary[PlayerStat.Exp].Init(exp);
        expGuage.fillAmount = exp / (float)expMax;
    }
    public void GainReward(RewardInfo rewardInfo)
    {
        switch (rewardInfo.id)
        {
            case 0:
                weaponContainers[rewardInfo.id].GetComponent<WeaponContainerA>().Add();
                statDictionary[PlayerStat.WeaponA].Increase();
                break;
            case 1:
                weaponContainers[rewardInfo.id].GetComponent<WeaponContainerB>().Add();
                statDictionary[PlayerStat.WeaponB].Increase();
                break;
            case 2:
                weaponContainers[rewardInfo.id].GetComponent<WeaponContainerC>().Add();
                statDictionary[PlayerStat.WeaponC].Increase();
                break;
            case 3:
                weaponContainers[rewardInfo.id].GetComponent<WeaponContainerD>().Add();
                statDictionary[PlayerStat.WeaponD].Increase();
                break;
            case 4:
                magnetCollider.radius *= 1.1f;
                statDictionary[PlayerStat.Magnet].Increase();
                break;
            case 5:
                speed *= 1.1f;
                statDictionary[PlayerStat.Speed].Increase();
                break;
            case 6:
                statDictionary[PlayerStat.Strength].Increase();
                break;
            case 7:
                statDictionary[PlayerStat.Life].Increase();
                break;
        }
    }
    public virtual void OnDamage(int damage)
    {
        int hp = statDictionary[PlayerStat.Hp].value;
        int hpMax = statDictionary[PlayerStat.HpMax].value;

        hp -= damage;

        if (hp > 0)
        {
            canvasGO.SetActive(true);
            hpGuage.fillAmount = (float)hp / (float)hpMax;

            animator.SetTrigger("doHit");

            statDictionary[PlayerStat.Hp].Init(hp);
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

    [System.Serializable]
    public class StatSlot
    {
        public PlayerStat type;
        public TextMeshProUGUI TextUI;
        public int value { get; private set; }

        public void Init(int value)
        {
            this.value = value;

            if (TextUI != null) 
            {
                TextUI.text = this.value.ToString();
            } 
        }
        public void Increase(int value = 1)
        {
            this.value += value;

            if (TextUI != null)
            {
                TextUI.text = this.value.ToString();
            }
        }
    }
    public enum PlayerStat 
    { 
        Level, 
        Kill, 
        Life, 
        Strength, 
        Speed, 
        Magnet, 
        WeaponA, 
        WeaponB, 
        WeaponC, 
        WeaponD,
        Hp,
        HpMax,
        Exp,
        ExpMax
    }
}
