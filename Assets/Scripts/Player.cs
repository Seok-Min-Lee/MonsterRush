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

    [SerializeField] private WeaponControllerBase[] weaponControllers;
    [SerializeField] private Transform magnetArea;
    [SerializeField] private Transform characterArea;
    [SerializeField] private ParticleSystem healParticle;

    [SerializeField] private float speed = 0.025f;

    [Header("UI")]
    [SerializeField] private VariableJoystick joystick;
    [SerializeField] private GameObject canvasGO;
    [SerializeField] private RectTransform stateToggleGroup;
    [SerializeField] private Image hpGuage;
    [SerializeField] private Image expGuage;
    [SerializeField] private StatSlot[] statSlots;
    [SerializeField] private StateToggle magnetToggle;
    [SerializeField] private Image healGuage;
    [SerializeField] private Transform buffIconStack;
    [SerializeField] private GameObject[] buffIcons;

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
    public bool IsDead => isDead;

    public Vector3 moveVec { get; private set; }
    public PlayerCharacter character { get; private set; }

    private BoxCollider2D collider;
    private SpriteRenderer magnetRenderer;

    private bool isDead = false;
    private bool isMagnetVisible = true;
    private void Awake()
    {
        Instance = this;

        collider = GetComponent<BoxCollider2D>();

        // 캐릭터
        PlayerCharacter prefab = Resources.Load<PlayerCharacter>("Players/Character_" + StaticValues.playerCharacterNum);
        character = GameObject.Instantiate<PlayerCharacter>(prefab, characterArea);
        character.Init(Vector3.zero);

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
                weaponControllers[0].Strengthen(0);
                statDictionary[PlayerStat.WeaponA].Increase();
                statDictionary[PlayerStat.Strength].Increase();
                break;
            case 1:
                weaponControllers[1].Strengthen(0);
                statDictionary[PlayerStat.WeaponB].Increase();
                statDictionary[PlayerStat.Speed].Increase();
                break;
            case 2:
                weaponControllers[2].Strengthen(0);
                statDictionary[PlayerStat.WeaponC].Increase();
                statDictionary[PlayerStat.Magnet].Increase();
                break;
            case 3:
                weaponControllers[3].Strengthen(0);
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
        character.UpdateTick(Time.deltaTime);

        if (isDead || Heal == 0 || StaticValues.isWait) return;

        if (Hp < HpMax)
        {
            if (healTimer > 10f)
            {
                int value = Hp + Heal > HpMax ? HpMax - Hp : Heal;
                healParticle.Play();
                statDictionary[PlayerStat.Hp].Increase(value);
                hpGuage.fillAmount = (float)Hp / HpMax;

                // 회복량 UI
                HitHealText hitText = UIContainer.Instance.Pop();
                Vector2 start = new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.3f, -0.2f));
                Vector2 target = start + new Vector2(0, 0.2f);
                hitText.Init(
                    value: "+" + value,
                    color: Color.green,
                    position: start,
                    target: target,
                    parent: canvasGO.transform
                );

                healTimer = 0f;
            }
            healGuage.fillAmount = healTimer / 10f;

            healTimer += Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        if (isDead || StaticValues.isWait) 
        {
            return;
        }
#if !UNITY_EDITOR
        moveVec = new Vector3(joystick.Horizontal, joystick.Vertical, 0f).normalized * speed;
        transform.position += moveVec;
        character.FlipX(moveVec.x < 0);

        float angle = Mathf.Atan2(joystick.Vertical, joystick.Horizontal) * Mathf.Rad2Deg;
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
    public void OnChangeUI(bool isLeftHand)
    {
        if (isLeftHand)
        {
            stateToggleGroup.anchorMin = new Vector2(1, 0);
            stateToggleGroup.anchorMax = new Vector2(1, 0);
            stateToggleGroup.pivot = new Vector2(1, 0);
            stateToggleGroup.anchoredPosition = new Vector2(-75, 260);
        }
        else
        {
            stateToggleGroup.anchorMin = new Vector2(0, 0);
            stateToggleGroup.anchorMax = new Vector2(0, 0);
            stateToggleGroup.pivot = new Vector2(0, 0);
            stateToggleGroup.anchoredPosition = new Vector2(275, -40);
        }
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
            if (Level == 80)
            {
                GameCtrl.Instance.OnGameEnd(GameResult.Clear);
            }
            else
            {
                GameCtrl.Instance.OnLevelUp();
            }
        }

        statDictionary[PlayerStat.Exp].Init(exp);
        expGuage.fillAmount = exp / (float)expMax;
    }
    public void OnReward(RewardInfo rewardInfo)
    {
        switch (rewardInfo.groupId)
        {
            case 1000:
                if (rewardInfo.type == RewardInfo.Type.Weapon)
                {
                    statDictionary[PlayerStat.WeaponA].Increase();
                }
                else
                {
                    GameObject.Instantiate(buffIcons[0], buffIconStack);
                }
                weaponControllers[0].Strengthen(rewardInfo.index);
                break;
            case 2000:
                if (rewardInfo.type == RewardInfo.Type.Weapon)
                {
                    statDictionary[PlayerStat.WeaponB].Increase();
                }
                else
                {
                    GameObject.Instantiate(buffIcons[1], buffIconStack);
                }
                weaponControllers[1].Strengthen(rewardInfo.index);
                break;
            case 3000:
                if (rewardInfo.type == RewardInfo.Type.Weapon)
                {
                    statDictionary[PlayerStat.WeaponC].Increase();
                }
                else
                {
                    GameObject.Instantiate(buffIcons[2], buffIconStack);
                }
                weaponControllers[2].Strengthen(rewardInfo.index);
                break;
            case 4000:
                if (rewardInfo.type == RewardInfo.Type.Weapon)
                {
                    statDictionary[PlayerStat.WeaponD].Increase();
                }
                else
                {
                    GameObject.Instantiate(buffIcons[3], buffIconStack);
                }
                weaponControllers[3].Strengthen(rewardInfo.index);
                break;
            case 0:
                switch (rewardInfo.index)
                {
                    case 0:
                        statDictionary[PlayerStat.Heal].Increase();
                        break;
                    case 1:
                        Vector3 scale = magnetArea.localScale + Vector3.one * 0.2f;
                        magnetArea.DOScale(scale, 0.5f);
                        statDictionary[PlayerStat.Magnet].Increase();
                        break;
                    case 2:
                        speed += 0.0025f;
                        statDictionary[PlayerStat.Speed].Increase();
                        break;
                    case 3:
                        statDictionary[PlayerStat.Strength].Increase();
                        break;
                }
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

            for (int i = 0; i < weaponControllers.Length; i++)
            {
                weaponControllers[i].gameObject.SetActive(false);
            }

            canvasGO.SetActive(false);
        };

        // death 연출
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(callback);
        seq.AppendInterval(0.75f);
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

        // 데미지 UI
        HitHealText hitText = UIContainer.Instance.Pop();
        Vector2 start = new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.3f, -0.2f));
        Vector2 target = start + new Vector2(0, 0.2f);
        hitText.Init(
            value: "-" + damage,
            color: Color.red,
            position: start,
            target: target,
            parent: canvasGO.transform
        );

        if (hp > 0)
        {
            canvasGO.SetActive(true);
            hpGuage.fillAmount = (float)hp / hpMax;

            character.PlayAnimation(PlayerCharacter.AniType.Hit);

            statDictionary[PlayerStat.Hp].Init(hp);
        }
        else
        {
            OnDeath();
            GameCtrl.Instance.OnGameEnd(Level < 80 ? GameResult.Defeat : GameResult.Challenge);
        }

        healTimer = 0f;
    }
    private void OnMove(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();

        if (v.sqrMagnitude > 0.001f)
        {
            moveVec = new Vector3(v.x, v.y, 0f) * speed;
            character.PlayAnimation(PlayerCharacter.AniType.Move);

            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

            character.FlipX(moveVec.x < 0);
        }
        else
        {
            moveVec = Vector3.zero;
            character.PlayAnimation(PlayerCharacter.AniType.Idle);
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
