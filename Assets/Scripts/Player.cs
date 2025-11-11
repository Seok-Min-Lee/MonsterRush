using DG.Tweening;
using System.Collections;
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
    [SerializeField] private Transform magnetArea;
    [SerializeField] private Transform characterArea;
    [SerializeField] private ParticleSystem healParticle;

    [SerializeField] private float speed = 0.025f;

    [Header("UI")]
    [SerializeField] private VariableJoystick joystick;
    [SerializeField] private GameObject canvasGO;
    [SerializeField] private CanvasGroup overlayCanvasCG;
    [SerializeField] private Image hpGuage;
    [SerializeField] private Image expGuage;
    [SerializeField] private StatSlot[] statSlots;
    [SerializeField] private StateToggle magnetToggle;

    private Dictionary<PlayerStat, StatSlot> statDictionary;

    public int Level => statDictionary[PlayerStat.Level].value;
    public int killCount => statDictionary[PlayerStat.Kill].value;
    public int Strength => statDictionary[PlayerStat.Strength].value;
    public int Heal => statDictionary[PlayerStat.Heal].value;
    public int Hp => statDictionary[PlayerStat.Hp].value;
    public int HpMax => statDictionary[PlayerStat.HpMax].value;
    public int Exp => statDictionary[PlayerStat.Exp].value;
    public int ExpMax => statDictionary[PlayerStat.ExpMax].value;
    public int weaponALevel => statDictionary[PlayerStat.WeaponA].value;
    public int weaponBLevel => statDictionary[PlayerStat.WeaponB].value;
    public int weaponCLevel => statDictionary[PlayerStat.WeaponC].value;
    public int weaponDLevel => statDictionary[PlayerStat.WeaponD].value;

    public Vector3 moveVec { get; private set; }
    public Animator animator { get; private set; }

    private BoxCollider2D collider;
    private SpriteRenderer characterRenderer;
    private SpriteRenderer magnetRenderer;

    private bool isDead = false;
    private bool isMagnetVisible = true;
    private void Awake()
    {
        Instance = this;

        collider = GetComponent<BoxCollider2D>();

        // 캐릭터
        GameObject characterPrefab = Resources.Load<GameObject>("Players/Character_" + StaticValues.playerCharacterNum);
        GameObject character = GameObject.Instantiate(characterPrefab, characterArea);
        character.transform.position = Vector3.zero;

        animator = character.GetComponent<Animator>();
        characterRenderer = character.GetComponent<SpriteRenderer>();

        // 자석
        magnetRenderer = magnetArea.GetComponent<SpriteRenderer>();
        magnetRenderer.enabled = isMagnetVisible;
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
        statDictionary[PlayerStat.Magnet].Init(0);
        statDictionary[PlayerStat.Speed].Init(0);
        statDictionary[PlayerStat.Strength].Init(0);
        statDictionary[PlayerStat.Heal].Init(0);
        statDictionary[PlayerStat.WeaponA].Init(0);
        statDictionary[PlayerStat.WeaponB].Init(0);
        statDictionary[PlayerStat.WeaponC].Init(0);
        statDictionary[PlayerStat.WeaponD].Init(0);
        statDictionary[PlayerStat.Hp].Init(100);
        statDictionary[PlayerStat.HpMax].Init(100);
        statDictionary[PlayerStat.Exp].Init(0);
        statDictionary[PlayerStat.ExpMax].Init(10);

        switch (StaticValues.playerCharacterNum)
        {
            case 0:
                weaponContainers[0].GetComponent<WeaponContainerA>().StrengthenFirst();
                statDictionary[PlayerStat.WeaponA].Increase();
                statDictionary[PlayerStat.Strength].Increase();
                break;
            case 1:
                weaponContainers[1].GetComponent<WeaponContainerB>().StrengthenFirst();
                statDictionary[PlayerStat.WeaponB].Increase();
                statDictionary[PlayerStat.Speed].Increase();
                break;
            case 2:
                weaponContainers[2].GetComponent<WeaponContainerC>().StrengthenFirst();
                statDictionary[PlayerStat.WeaponC].Increase();
                statDictionary[PlayerStat.Magnet].Increase();
                break;
            case 3:
                weaponContainers[3].GetComponent<WeaponContainerD>().StrengthenFirst();
                statDictionary[PlayerStat.WeaponD].Increase();
                statDictionary[PlayerStat.Heal].Increase();
                break;
            default:
                break;
        }

        hpGuage.fillAmount = 1f;
        expGuage.fillAmount = 0f;

        magnetToggle.Init(isMagnetVisible);
    }
    private float healTimer = 0f;
    private void Update()
    {
        if (isDead || Heal == 0) return;

        if (Hp < HpMax)
        {
            if (healTimer > 10f)
            {
                int value = Hp + Heal > HpMax ? HpMax - Hp : Heal;
                healParticle.Play();
                statDictionary[PlayerStat.Hp].Increase(value);
                hpGuage.fillAmount = (float)Hp / HpMax;

                healTimer = 0f;
            }

            healTimer += Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        if (isDead) 
        {
            return;
        }
#if !UNITY_EDITOR
        moveVec = new Vector3(joystick.Horizontal, joystick.Vertical, 0f).normalized * speed;
        transform.position += moveVec;
        characterRenderer.flipX = moveVec.x < 0;

        float angle = Mathf.Atan2(joystick.Vertical, joystick.Horizontal) * Mathf.Rad2Deg;
        weaponContainers[1].rotation = Quaternion.Euler(0, 0, angle);
#else
        transform.position += moveVec;
#endif
    }
    public void OnClickMagnetVisibility()
    {
        AudioManager.Instance.PlaySFX(SoundKey.GameTouch);

        isMagnetVisible = !isMagnetVisible;
        magnetRenderer.enabled = isMagnetVisible;
        magnetToggle.Init(isMagnetVisible);
    }
    public void OnClickLevelUp()
    {
        IncreaseExp(ExpMax);
    }
    public void KillEnemy()
    {
        statDictionary[PlayerStat.Kill].Increase();
    }
    public void IncreaseExp(int value)
    {
        int exp = Exp;
        int expMax = ExpMax;

        exp += value;

        if (exp >= expMax)
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
            AudioManager.Instance.PlaySFX(SoundKey.PlayerLevelUp);
        }

        statDictionary[PlayerStat.Exp].Init(exp);
        expGuage.fillAmount = exp / (float)expMax;
    }
    public void OnReward(RewardInfo rewardInfo)
    {
        switch (rewardInfo.id)
        {
            case 0:
                statDictionary[PlayerStat.WeaponA].Increase();
                weaponContainers[0].GetComponent<WeaponContainerA>().StrengthenFirst();
                break;
            case 1:
                statDictionary[PlayerStat.WeaponA].Increase();
                weaponContainers[0].GetComponent<WeaponContainerA>().StrengthenSecond();
                break;
            case 10:
                statDictionary[PlayerStat.WeaponB].Increase();
                weaponContainers[1].GetComponent<WeaponContainerB>().StrengthenFirst();
                break;
            case 11:
                statDictionary[PlayerStat.WeaponB].Increase();
                weaponContainers[1].GetComponent<WeaponContainerB>().StrengthenSecond();
                break;
            case 20:
                statDictionary[PlayerStat.WeaponC].Increase();
                weaponContainers[2].GetComponent<WeaponContainerC>().StrengthenFirst();
                break;
            case 21:
                statDictionary[PlayerStat.WeaponC].Increase();
                weaponContainers[2].GetComponent<WeaponContainerC>().StrengthenSecond();
                break;
            case 30:
                statDictionary[PlayerStat.WeaponD].Increase();
                weaponContainers[3].GetComponent<WeaponContainerD>().StrengthenFirst();
                break;
            case 31:
                statDictionary[PlayerStat.WeaponD].Increase();
                weaponContainers[3].GetComponent<WeaponContainerD>().StrengthenSecond();
                break;
            case 90:
                statDictionary[PlayerStat.Heal].Increase();
                break;
            case 91:
                Vector3 scale = magnetArea.localScale * 1.1f;
                magnetArea.DOScale(scale, 0.5f);
                statDictionary[PlayerStat.Magnet].Increase();
                break;
            case 92:
                speed *= 1.1f;
                statDictionary[PlayerStat.Speed].Increase();
                break;
            case 93:
                statDictionary[PlayerStat.Strength].Increase();
                break;
        }
    }
    public void OnDeath()
    {
        // death 처리 로직
        TweenCallback callback = () => 
        {
            isDead = true;
            collider.enabled = false;

            isMagnetVisible = false;
            magnetRenderer.enabled = isMagnetVisible;

            for (int i = 0; i < weaponContainers.Length; i++)
            {
                weaponContainers[i].gameObject.SetActive(false);
            }

            canvasGO.SetActive(false);
        };

        // death 연출
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(callback);
        seq.Append(overlayCanvasCG.DOFade(0f, 0.5f));
        seq.AppendInterval(0.25f);
        seq.Append(characterArea.DORotate(new Vector3(0, 0, 90), 0.5f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.25f);
        seq.Append(characterArea.DOScaleX(0f, 0.5f));
    }
    public void OnDamage(int damage)
    {
        if (isDead) 
        {
            return;
        }
        
        int hp = statDictionary[PlayerStat.Hp].value;
        int hpMax = statDictionary[PlayerStat.HpMax].value;

        hp -= damage;

        if (hp > 0)
        {
            canvasGO.SetActive(true);
            hpGuage.fillAmount = (float)hp / hpMax;

            animator.SetTrigger("doHit");

            statDictionary[PlayerStat.Hp].Init(hp);
        }
        else
        {
            GameCtrl.Instance.OnGameEnd();
        }

        healTimer = 0f;
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

            characterRenderer.flipX = moveVec.x < 0;
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
        public int maxValue = int.MaxValue;
        public int value { get; private set; }

        public void Init(int value)
        {
            this.value = Mathf.Min(value, maxValue);

            if (TextUI != null)
            {
                TextUI.text = this.value == maxValue ? "MAX" : this.value.ToString();
            } 
        }
        public void Increase(int value = 1)
        {
            this.value = Mathf.Min(this.value + value, maxValue);

            if (TextUI != null)
            {
                TextUI.text = this.value == maxValue ? "MAX" : this.value.ToString();
            }
        }
        public bool TryIncrease(int value = 1)
        {
            if (this.value < maxValue)
            {
                Increase(value);

                return true;
            }

            return false;
        }
    }
    public enum PlayerStat 
    { 
        Level, 
        Kill, 
        Heal, 
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
