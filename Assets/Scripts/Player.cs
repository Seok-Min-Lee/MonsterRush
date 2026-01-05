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
    private const int HP_MAX_INCREASEMENT_UNIT = 5;
    private const float BARRIER_DURATION = 10F;
    private const float EXP_BOOST_DURATION = 30F;
    private const float POWER_UP_DURATION = 10F;

    private readonly string COMBAT_TEXT_INCREASE_HP = $"최대 HP 증가 +{HP_MAX_INCREASEMENT_UNIT}";
    private readonly string COMBAT_TEXT_CANCEL = "0";
    private readonly string COMBAT_TEXT_EXP_BOOST = $"경험치 부스트: {EXP_BOOST_DURATION}초간 경험치 증가";
    private readonly string COMBAT_TEXT_POWER_UP = $"강화 알약: {POWER_UP_DURATION}초간 공격력 증가";
    private readonly string COMBAT_TEXT_BARRIER = $"방어막: {BARRIER_DURATION}초간 피격 무효화";

    [SerializeField] private WeaponControllerBase[] weaponControllers;
    [SerializeField] private Transform magnetArea;
    [SerializeField] private Transform characterArea;
    [SerializeField] private ParticleSystem healParticle;
    [SerializeField] private ParticleSystem barrierParticle;

    [SerializeField] private float speed = 0.025f;

    [Header("UI")]
    [SerializeField] private VariableJoystick joystick;
    [SerializeField] private GameObject hpBar;
    [SerializeField] private Transform hpGuage;
    [SerializeField] private RectTransform stateToggleGroup;
    [SerializeField] private Image expGuage;
    [SerializeField] private StatSlot[] statSlots;
    [SerializeField] private StateToggle magnetToggle;
    [SerializeField] private Image healGuage;
    [SerializeField] private Transform playerAbilityIconGroup;
    [SerializeField] private GameObject[] playerAbilityIcons;
    [SerializeField] private Transform weaponAbilityIconGroup;
    [SerializeField] private GameObject[] weaponAbilityIcons;
    [SerializeField] private Transform[] buffStackChildren;
    [SerializeField] private TextMeshPro[] buffStackTexts;

    public int Level => statSlots[(int)PlayerStat.Level].Value;
    public int killCount => statSlots[(int)PlayerStat.Kill].Value;
    public int Heal => statSlots[(int)PlayerStat.Heal].Value;
    public int Magnet => statSlots[(int)PlayerStat.Magnet].Value;
    public int Speed => statSlots[(int)PlayerStat.Speed].Value;
    public int Strength => isPowerUp ?
        statSlots[(int)PlayerStat.Strength].Value + 10 :
        statSlots[(int)PlayerStat.Strength].Value;
    public int Hp => statSlots[(int)PlayerStat.Hp].Value;
    public int HpMax => statSlots[(int)PlayerStat.HpMax].Value;
    public int Exp => statSlots[(int)PlayerStat.Exp].Value;
    public int ExpMax => statSlots[(int)PlayerStat.ExpMax].Value;
    public int weaponALevel => statSlots[(int)PlayerStat.WeaponA].Value;
    public int weaponBLevel => statSlots[(int)PlayerStat.WeaponB].Value;
    public int weaponCLevel => statSlots[(int)PlayerStat.WeaponC].Value;
    public int weaponDLevel => statSlots[(int)PlayerStat.WeaponD].Value;
    public bool IsDead => isDead;
    public bool IsAvailableSecondEnhance => isAvailableSecondEnhance;
    public int[] PlayerAbilities => playerAbilities.ToArray();
    public int[] WeaponAbilities => weaponAbilities.ToArray();

    public Vector3 moveVec { get; private set; }
    public PlayerCharacter character { get; private set; }

    private BoxCollider2D collider;
    private SpriteRenderer magnetRenderer;

    private bool isDead = false;
    private bool isMagnetVisible = true;
    private bool isAvailableSecondEnhance = false;
    private bool isAvailableExtraExp = false;
    private bool isBarrier = false;
    private bool isExpBoost = false;
    private bool isPowerUp = false;
    private float barrierTimer = 0f;
    private float expBoostTimer = 0f;
    private float powerUpTimer = 0f;
    private int plusHp = 0;
    private int extraExp = 1;

    private List<int> weaponAbilities = new List<int>();
    private List<int> playerAbilities = new List<int>();
    private List<Vector3> buffStackPositions = new List<Vector3>();
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

        //
        for (int i = 0; i < buffStackChildren.Length; i++)
        {
            buffStackPositions.Add(buffStackChildren[i].localPosition);
            buffStackChildren[i].gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        hpBar.SetActive(false);

        // 스탯 초기화
        statSlots[(int)PlayerStat.Level].InitValue(1);
        statSlots[(int)PlayerStat.Kill].InitValue(0);
        statSlots[(int)PlayerStat.Magnet].InitValue(0);
        statSlots[(int)PlayerStat.Speed].InitValue(0);
        statSlots[(int)PlayerStat.Strength].InitValue(0);
        statSlots[(int)PlayerStat.Heal].InitValue(0);
        statSlots[(int)PlayerStat.WeaponA].InitValue(0);
        statSlots[(int)PlayerStat.WeaponB].InitValue(0);
        statSlots[(int)PlayerStat.WeaponC].InitValue(0);
        statSlots[(int)PlayerStat.WeaponD].InitValue(0);
        statSlots[(int)PlayerStat.Hp].InitValue(100);
        statSlots[(int)PlayerStat.HpMax].InitValue(100);
        statSlots[(int)PlayerStat.Exp].InitValue(0);
        statSlots[(int)PlayerStat.ExpMax].InitValue(10);

        switch (StaticValues.playerCharacterNum)
        {
            case 0:
                weaponControllers[0].Strengthen(0);
                statSlots[(int)PlayerStat.WeaponA].Increase();
                statSlots[(int)PlayerStat.Strength].Increase();
                break;
            case 1:
                weaponControllers[1].Strengthen(0);
                statSlots[(int)PlayerStat.WeaponB].Increase();
                statSlots[(int)PlayerStat.Speed].Increase();
                break;
            case 2:
                weaponControllers[2].Strengthen(0);
                statSlots[(int)PlayerStat.WeaponC].Increase();
                statSlots[(int)PlayerStat.Magnet].Increase();
                break;
            case 3:
                weaponControllers[3].Strengthen(0);
                statSlots[(int)PlayerStat.WeaponD].Increase();
                statSlots[(int)PlayerStat.Heal].Increase();
                break;
            default:
                break;
        }

        hpGuage.localScale = Vector3.one;
        expGuage.fillAmount = 0f;

        magnetToggle.Init(isMagnetVisible);
    }
    private float healTimer = 0f;
    private float healCooltime = 10f;
    private void Update()
    {
        character.UpdateTick(Time.deltaTime);

        if (isDead || Heal == 0 || StaticValues.isWait) return;

        if (Hp < HpMax)
        {
            if (healTimer > healCooltime)
            {
                OnHeal(Heal);

                healTimer = 0f;
            }
            healGuage.fillAmount = healTimer / healCooltime;

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
    public void IncreaseKill()
    {
        if (isAvailableExtraExp)
        {
            IncreaseExp(extraExp);
        }

        statSlots[(int)PlayerStat.Kill].Increase();
    }
    public void IncreaseHp(int value)
    {
        OnHeal(value);
        statSlots[(int)PlayerStat.HpMax].Increase(value);
        ShowCombatText(
            type: CombatText.Type.StateChange,
            text: COMBAT_TEXT_INCREASE_HP
        );

        plusHp += value;
        buffStackTexts[0].text = $"{(int)plusHp}";
        if (!buffStackChildren[0].gameObject.activeSelf)
        {
            buffStackChildren[0].gameObject.SetActive(true);
            BuffStackRebuild();
        }
    }
    public void IncreaseExp(int value)
    {
        int exp = Exp;
        int expMax = ExpMax;

        exp += isExpBoost ? (int)(value * 1.5) : value;

        if (exp >= expMax)
        {
            exp -= expMax;
            expMax = (int)(expMax * 1.1f);

            statSlots[(int)PlayerStat.Level].Increase();
            statSlots[(int)PlayerStat.ExpMax].InitValue(expMax);

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

            extraExp = (int)(Level * 0.2f);
        }

        statSlots[(int)PlayerStat.Exp].InitValue(exp);
        expGuage.fillAmount = exp / (float)expMax;
    }
    public void OnReward(RewardInfo rewardInfo)
    {
        switch (rewardInfo.groupId)
        {
            case 1000:
                if (rewardInfo.type == RewardInfo.Type.Weapon)
                {
                    statSlots[(int)PlayerStat.WeaponA].Increase();
                }
                else
                {
                    GameObject.Instantiate(weaponAbilityIcons[0], weaponAbilityIconGroup);
                    weaponAbilities.Add(0);
                }
                weaponControllers[0].Strengthen(rewardInfo.index);
                break;
            case 2000:
                if (rewardInfo.type == RewardInfo.Type.Weapon)
                {
                    statSlots[(int)PlayerStat.WeaponB].Increase();
                }
                else
                {
                    GameObject.Instantiate(weaponAbilityIcons[1], weaponAbilityIconGroup);
                    weaponAbilities.Add(1);
                }
                weaponControllers[1].Strengthen(rewardInfo.index);
                break;
            case 3000:
                if (rewardInfo.type == RewardInfo.Type.Weapon)
                {
                    statSlots[(int)PlayerStat.WeaponC].Increase();
                }
                else
                {
                    GameObject.Instantiate(weaponAbilityIcons[2], weaponAbilityIconGroup);
                    weaponAbilities.Add(2);
                }
                weaponControllers[2].Strengthen(rewardInfo.index);
                break;
            case 4000:
                if (rewardInfo.type == RewardInfo.Type.Weapon)
                {
                    statSlots[(int)PlayerStat.WeaponD].Increase();
                }
                else
                {
                    GameObject.Instantiate(weaponAbilityIcons[3], weaponAbilityIconGroup);
                    weaponAbilities.Add(3);
                }
                weaponControllers[3].Strengthen(rewardInfo.index);
                break;
            case 0:
                switch (rewardInfo.index)
                {
                    case 0:
                        statSlots[(int)PlayerStat.Heal].Increase();
                        break;
                    case 1:
                        Vector3 scale = magnetArea.localScale + Vector3.one * 0.2f;
                        magnetArea.DOScale(scale, 0.5f);
                        statSlots[(int)PlayerStat.Magnet].Increase();
                        break;
                    case 2:
                        speed += 0.0025f;
                        statSlots[(int)PlayerStat.Speed].Increase();
                        break;
                    case 3:
                        statSlots[(int)PlayerStat.Strength].Increase();
                        break;
                    case 96:
                        GameObject.Instantiate(playerAbilityIcons[0], playerAbilityIconGroup);
                        playerAbilities.Add(0);
                        break;
                    case 97:
                        GameObject.Instantiate(playerAbilityIcons[1], playerAbilityIconGroup);
                        playerAbilities.Add(1);
                        isAvailableExtraExp = true;
                        break;
                    case 98:
                        GameObject.Instantiate(playerAbilityIcons[2], playerAbilityIconGroup);
                        playerAbilities.Add(2);

                        statSlots[(int)PlayerStat.WeaponA].InitMaxValue(16);
                        statSlots[(int)PlayerStat.WeaponB].InitMaxValue(16);
                        statSlots[(int)PlayerStat.WeaponC].InitMaxValue(16);
                        statSlots[(int)PlayerStat.WeaponD].InitMaxValue(16);
                        isAvailableSecondEnhance = true;
                        break;
                    case 99:
                        GameObject.Instantiate(playerAbilityIcons[3], playerAbilityIconGroup);
                        playerAbilities.Add(3);
                        healCooltime *= 0.5f;
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

            hpBar.SetActive(false);
            for (int i = 0; i < buffStackChildren.Length; i++)
            {
                buffStackChildren[i].gameObject.SetActive(false);
            }
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

        if (isBarrier)
        {
            AudioManager.Instance.PlaySFX(SoundKey.BarrierHit);

            ShowCombatText(
                type: CombatText.Type.Cancel,
                text: COMBAT_TEXT_CANCEL
            );

            return;
        }

        AudioManager.Instance.PlaySFX(SoundKey.PlayerHit);

        int hp = statSlots[(int)PlayerStat.Hp].Value;
        hp -= damage;

        ShowCombatText(
            type: CombatText.Type.Damage,
            text: "-" + damage
        );

        if (hp > 0)
        {
            hpBar.SetActive(true);
            hpGuage.localScale = new Vector3((float)Hp / HpMax, 1f, 1f);

            character.PlayAnimation(PlayerCharacter.AniType.Hit);

            statSlots[(int)PlayerStat.Hp].InitValue(hp);
        }
        else
        {
            OnDeath();
            GameCtrl.Instance.OnGameEnd(Level < 80 ? GameResult.Defeat : GameResult.Challenge);
        }

        healTimer = 0f;
    }
    public void OnHeal(int value)
    {
        if (isDead)
        {
            return;
        }

        int amount = Mathf.Clamp(value, 0, HpMax - Hp);

        if (amount > 0)
        {
            healParticle.Play();
            statSlots[(int)PlayerStat.Hp].Increase(amount);
            hpGuage.localScale = new Vector3((float)Hp / HpMax, 1f, 1f);

            if (Hp >= HpMax)
            {
                healTimer = 0f;
                healGuage.fillAmount = 0f;
            }
        }

        ShowCombatText(
            type: amount > 0 ? CombatText.Type.Heal: CombatText.Type.Cancel,
            text: $"+{amount}"
        );
    }
    public void OnBarrier()
    {
        if (isBarrier)
        {
            barrierTimer = 0f; 
            buffStackTexts[2].text = $"{(int)BARRIER_DURATION}";
        }
        else
        {
            StartCoroutine(OnBarrierCor());
        }
    }
    public void OnExpBoost()
    {
        if (isExpBoost)
        {
            expBoostTimer = 0f;
            buffStackTexts[1].text = $"{(int)EXP_BOOST_DURATION}";
        }
        else
        {
            StartCoroutine(OnExpBoostCor());
        }
    }
    public void OnPowerUp()
    {
        if (isPowerUp)
        {
            powerUpTimer = 0f;
            buffStackTexts[3].text = $"{(int)POWER_UP_DURATION}";
        }
        else
        {
            StartCoroutine(OnPowerUpCor());
        }
    }
    private void ShowCombatText(CombatText.Type type, string text)
    {
        CombatText combatText = UIContainer.Instance.Pop();
        combatText.Init(
            type: type,
            value: text,
            parent: transform
        );
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
    private WaitForSeconds WaitForSecond = new WaitForSeconds(1f);
    private IEnumerator OnBarrierCor()
    {
        isBarrier = true;
        barrierParticle.Play();

        ShowCombatText(
            type: CombatText.Type.StateChange,
            text: COMBAT_TEXT_BARRIER
        );

        buffStackChildren[2].gameObject.SetActive(true);
        buffStackTexts[2].text = $"{(int)BARRIER_DURATION}";
        BuffStackRebuild();

        barrierTimer = 0f;
        while (barrierTimer < BARRIER_DURATION)
        {
            yield return WaitForSecond;

            barrierTimer += 1;
            buffStackTexts[2].text = $"{(int)(BARRIER_DURATION - barrierTimer)}";
        }

        buffStackChildren[2].gameObject.SetActive(false);
        BuffStackRebuild();

        barrierParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        isBarrier = false;
    }
    private IEnumerator OnExpBoostCor()
    {
        isExpBoost = true;

        ShowCombatText(
            type: CombatText.Type.StateChange,
            text: COMBAT_TEXT_EXP_BOOST
        );

        buffStackChildren[1].gameObject.SetActive(true);
        buffStackTexts[1].text = $"{(int)EXP_BOOST_DURATION}";
        BuffStackRebuild();

        expBoostTimer = 0f;
        while (expBoostTimer < EXP_BOOST_DURATION)
        {
            yield return WaitForSecond;

            expBoostTimer += 1;
            buffStackTexts[1].text = $"{(int)(EXP_BOOST_DURATION - expBoostTimer)}";
        }

        buffStackChildren[1].gameObject.SetActive(false);
        BuffStackRebuild();

        isExpBoost = false;
    }
    private IEnumerator OnPowerUpCor()
    {
        isPowerUp = true;

        ShowCombatText(
            type: CombatText.Type.StateChange,
            text: COMBAT_TEXT_POWER_UP
        );

        buffStackChildren[3].gameObject.SetActive(true);
        buffStackTexts[3].text = $"{(int)POWER_UP_DURATION}";
        BuffStackRebuild();

        powerUpTimer = 0f;
        while (powerUpTimer < POWER_UP_DURATION)
        {
            yield return WaitForSecond;

            powerUpTimer += 1;
            buffStackTexts[3].text = $"{(int)(POWER_UP_DURATION - powerUpTimer)}";
        }

        buffStackChildren[3].gameObject.SetActive(false);
        BuffStackRebuild();

        isPowerUp = false;
    }

    private void BuffStackRebuild()
    {
        int idx = 0;
        for (int i = 0; i < buffStackChildren.Length; i++)
        {
            Transform child = buffStackChildren[i];

            if (child.gameObject.activeSelf)
            {
                child.localPosition = buffStackPositions[idx++];
            }
        }
    }

    [System.Serializable]
    public class StatSlot
    {
        [SerializeField] private PlayerStat type;
        [SerializeField] private TMP_Text TextUI;
        [SerializeField] private int maxValue = int.MaxValue;
        [SerializeField] private int value;

        public PlayerStat Type => type;
        public int Value => value;
        public void InitValue(int value)
        {
            this.value = Mathf.Min(value, maxValue);

            UpdateUI();
        }
        public void InitMaxValue(int value)
        {
            maxValue = value;

            UpdateUI();
        }
        public void Increase(int value = 1)
        {
            this.value = Mathf.Min(this.value + value, maxValue);

            UpdateUI();
        }
        private void UpdateUI()
        {
            if (TextUI != null)
            {
                TextUI.text = value == maxValue ? "MAX" : value.ToString();
            }
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
