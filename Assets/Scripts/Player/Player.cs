using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

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
    [SerializeField] private float healCooltime = 10f;

    [Header("Canvas UI")]
    [SerializeField] private PlayerStat[] playerStats;
    [SerializeField] private Image expGaugeBar;
    [SerializeField] private AbilityStack playerAbilityStack;
    [SerializeField] private AbilityStack weaponAbilityStack;
    [SerializeField] private VariableJoystick joystick;
    [SerializeField] private CustomToggleGroup stateToggleGroup;

    [Header("World UI")]
    [SerializeField] private GaugeBar hpGaugeBar;
    [SerializeField] private BuffStack buffStack;

    public int Level => playerStats[(int)PlayerStat.Type.Level].Value;
    public int killCount => playerStats[(int)PlayerStat.Type.Kill].Value;
    public int Heal => playerStats[(int)PlayerStat.Type.Heal].Value;
    public int Magnet => playerStats[(int)PlayerStat.Type.Magnet].Value;
    public int Speed => playerStats[(int)PlayerStat.Type.Speed].Value;
    public int Strength => playerStats[(int)PlayerStat.Type.Strength].Value;
    public int Hp => playerStats[(int)PlayerStat.Type.Hp].Value;
    public int HpMax => playerStats[(int)PlayerStat.Type.HpMax].Value;
    public int Exp => playerStats[(int)PlayerStat.Type.Exp].Value;
    public int ExpMax => playerStats[(int)PlayerStat.Type.ExpMax].Value;
    public int WeaponALevel => playerStats[(int)PlayerStat.Type.WeaponA].Value;
    public int WeaponBLevel => playerStats[(int)PlayerStat.Type.WeaponB].Value;
    public int WeaponCLevel => playerStats[(int)PlayerStat.Type.WeaponC].Value;
    public int WeaponDLevel => playerStats[(int)PlayerStat.Type.WeaponD].Value;
    public int AdditionalDamage => Strength + (isPowerUp ? 10 : 0);

    public PlayerCharacter Character { get; private set; }
    public List<int> WeaponAbilities { get; private set; } = new List<int>();
    public List<int> PlayerAbilities { get; private set; } = new List<int>();
    public Vector3 MoveVec { get; private set; }
    public bool IsDead { get; private set; } = false;
    public bool IsAvailableSecondEnhance { get; private set; } = false;

    private BoxCollider2D collider;

    private bool isAvailableExtraExp = false;

    // buff
    private bool isBarrier = false;
    private bool isExpBoost = false;
    private bool isPowerUp = false;
    private float barrierTimer = 0f;
    private float expBoostTimer = 0f;
    private float powerUpTimer = 0f;
    private int plusHp = 0;
    private int extraExp = 1;

    private float healTimer = 0f;
    private WaitForSeconds WaitForSecond = new WaitForSeconds(1f);
    private void Awake()
    {
        Instance = this;

        collider = GetComponent<BoxCollider2D>();

        // 캐릭터
        PlayerCharacter prefab = Resources.Load<PlayerCharacter>("Players/Character_" + StaticValues.playerCharacterNum);
        Character = GameObject.Instantiate<PlayerCharacter>(prefab, characterArea);
        Character.Init(Vector3.zero);
    }
    private void Start()
    {
        // 스탯 초기화
        playerStats[(int)PlayerStat.Type.Level].Init(1, int.MaxValue);
        playerStats[(int)PlayerStat.Type.Kill].Init(0, int.MaxValue);
        playerStats[(int)PlayerStat.Type.Magnet].Init(0, int.MaxValue);
        playerStats[(int)PlayerStat.Type.Speed].Init(0, int.MaxValue);
        playerStats[(int)PlayerStat.Type.Strength].Init(0, int.MaxValue);
        playerStats[(int)PlayerStat.Type.Heal].Init(0, int.MaxValue);
        playerStats[(int)PlayerStat.Type.WeaponA].Init(0, 8);
        playerStats[(int)PlayerStat.Type.WeaponB].Init(0, 8);
        playerStats[(int)PlayerStat.Type.WeaponC].Init(0, 8);
        playerStats[(int)PlayerStat.Type.WeaponD].Init(0, 8);
        playerStats[(int)PlayerStat.Type.Hp].Init(10000, int.MaxValue);
        playerStats[(int)PlayerStat.Type.HpMax].Init(10000, int.MaxValue);
        playerStats[(int)PlayerStat.Type.Exp].Init(0, int.MaxValue);  
        playerStats[(int)PlayerStat.Type.ExpMax].Init(10, int.MaxValue);

        switch (StaticValues.playerCharacterNum)
        {
            case 0:
                weaponControllers[0].Strengthen(0);
                playerStats[(int)PlayerStat.Type.WeaponA].IncreaseValue();
                playerStats[(int)PlayerStat.Type.Strength].IncreaseValue();
                break;
            case 1:
                weaponControllers[1].Strengthen(0);
                playerStats[(int)PlayerStat.Type.WeaponB].IncreaseValue();
                playerStats[(int)PlayerStat.Type.Speed].IncreaseValue();
                break;
            case 2:
                weaponControllers[2].Strengthen(0);
                playerStats[(int)PlayerStat.Type.WeaponC].IncreaseValue();
                playerStats[(int)PlayerStat.Type.Magnet].IncreaseValue();
                break;
            case 3:
                weaponControllers[3].Strengthen(0);
                playerStats[(int)PlayerStat.Type.WeaponD].IncreaseValue();
                playerStats[(int)PlayerStat.Type.Heal].IncreaseValue();
                break;
            default:
                break;
        }

        hpGaugeBar.Init(maxValue: HpMax, currentValue: Hp, visible: false);
        expGaugeBar.fillAmount = 0f;

        //
        stateToggleGroup.Init(isMagentVisible: true);
    }
    private void Update()
    {
        Character.UpdateTick(Time.deltaTime);

        if (IsDead || Heal == 0 || StaticValues.isWait) return;

        if (Hp < HpMax)
        {
            if (healTimer > healCooltime)
            {
                OnHeal(Heal);

                healTimer = 0f;
            }
            
            stateToggleGroup.HealGauge.fillAmount = healTimer / healCooltime;

            healTimer += Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        if (IsDead || StaticValues.isWait) 
        {
            return;
        }
#if !UNITY_EDITOR
        moveVec = new Vector3(joystick.Horizontal, joystick.Vertical, 0f).normalized * speed;
        transform.position += moveVec;
        character.FlipX(moveVec.x < 0);

        float angle = Mathf.Atan2(joystick.Vertical, joystick.Horizontal) * Mathf.Rad2Deg;
#else
        transform.position += MoveVec;
#endif
    }
    public void OnClickLevelUp()
    {
        IncreaseExp(ExpMax);
    }
    public void OnChangeUI(bool isLeftHand)
    {
        stateToggleGroup.ChangeUI(isLeftHand);
    }
    public void IncreaseKill()
    {
        if (isAvailableExtraExp)
        {
            IncreaseExp(extraExp);
        }

        playerStats[(int)PlayerStat.Type.Kill].IncreaseValue();
    }
    public void IncreaseHp(int value)
    {
        OnHeal(value);
        playerStats[(int)PlayerStat.Type.HpMax].IncreaseValue(value);
        ShowCombatText(
            type: CombatText.Type.StateChange,
            text: COMBAT_TEXT_INCREASE_HP
        );

        plusHp += value;
        buffStack.SetValue(0, $"{(int)plusHp}");
    }
    public void IncreaseExp(int value)
    {
        int exp = Exp;
        int expMax = ExpMax;

        exp += isExpBoost ? (int)(value * 1.5) : value;

        while (exp >= expMax)
        {
            // 레벨업 처리
            exp -= expMax;
            expMax = (int)(expMax * 1.1f);

            playerStats[(int)PlayerStat.Type.Level].IncreaseValue();
            playerStats[(int)PlayerStat.Type.ExpMax].SetValue(expMax);

            // 입력 초기화
            var pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
                position = Input.mousePosition
            };
            ExecuteEvents.Execute<IPointerUpHandler>(joystick.gameObject, pointerData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute<IEndDragHandler>(joystick.gameObject, pointerData, ExecuteEvents.endDragHandler);

            //
            if (Level == StaticValues.CHECKPOINT_LEVEL)
            {
                GameCtrl.Instance.OnGameEnd(GameResult.Clear);
            }
            else
            {
                GameCtrl.Instance.OnLevelUp();
            }

            extraExp = (int)(Level * 0.2f);
        }

        playerStats[(int)PlayerStat.Type.Exp].SetValue(exp);
        expGaugeBar.fillAmount = exp / (float)expMax;
    }
    public void OnReward(RewardInfo rewardInfo)
    {
        switch (rewardInfo.groupId)
        {
            case 1000:
                if (rewardInfo.type == RewardInfo.Type.Weapon)
                {
                    playerStats[(int)PlayerStat.Type.WeaponA].IncreaseValue();
                }
                else
                {
                    weaponAbilityStack.Push(0);
                    WeaponAbilities.Add(0);
                }
                weaponControllers[0].Strengthen(rewardInfo.index);
                break;
            case 2000:
                if (rewardInfo.type == RewardInfo.Type.Weapon)
                {
                    playerStats[(int)PlayerStat.Type.WeaponB].IncreaseValue();
                }
                else
                {
                    weaponAbilityStack.Push(1);
                    WeaponAbilities.Add(1);
                }
                weaponControllers[1].Strengthen(rewardInfo.index);
                break;
            case 3000:
                if (rewardInfo.type == RewardInfo.Type.Weapon)
                {
                    playerStats[(int)PlayerStat.Type.WeaponC].IncreaseValue();
                }
                else
                {
                    weaponAbilityStack.Push(2);
                    WeaponAbilities.Add(2);
                }
                weaponControllers[2].Strengthen(rewardInfo.index);
                break;
            case 4000:
                if (rewardInfo.type == RewardInfo.Type.Weapon)
                {
                    playerStats[(int)PlayerStat.Type.WeaponD].IncreaseValue();
                }
                else
                {
                    weaponAbilityStack.Push(3);
                    WeaponAbilities.Add(3);
                }
                weaponControllers[3].Strengthen(rewardInfo.index);
                break;
            case 0:
                switch (rewardInfo.index)
                {
                    case 0:
                        playerStats[(int)PlayerStat.Type.Heal].IncreaseValue();
                        break;
                    case 1:
                        Vector3 scale = magnetArea.localScale + Vector3.one * 0.2f;
                        magnetArea.DOScale(scale, 0.5f);
                        playerStats[(int)PlayerStat.Type.Magnet].IncreaseValue();
                        break;
                    case 2:
                        speed += 0.0025f;
                        playerStats[(int)PlayerStat.Type.Speed].IncreaseValue();
                        break;
                    case 3:
                        playerStats[(int)PlayerStat.Type.Strength].IncreaseValue();
                        break;
                    case 96:
                        playerAbilityStack.Push(0);
                        PlayerAbilities.Add(0);
                        break;
                    case 97:
                        playerAbilityStack.Push(1);
                        PlayerAbilities.Add(1);
                        isAvailableExtraExp = true;
                        break;
                    case 98:
                        playerAbilityStack.Push(2);
                        PlayerAbilities.Add(2);

                        playerStats[(int)PlayerStat.Type.WeaponA].SetMaxValue(16);
                        playerStats[(int)PlayerStat.Type.WeaponB].SetMaxValue(16);
                        playerStats[(int)PlayerStat.Type.WeaponC].SetMaxValue(16);
                        playerStats[(int)PlayerStat.Type.WeaponD].SetMaxValue(16);
                        IsAvailableSecondEnhance = true;
                        break;
                    case 99:
                        playerAbilityStack.Push(3);
                        PlayerAbilities.Add(3);
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
            IsDead = true;
            collider.enabled = false;

            stateToggleGroup.Init(isMagentVisible: false);

            for (int i = 0; i < weaponControllers.Length; i++)
            {
                weaponControllers[i].gameObject.SetActive(false);
            }

            hpGaugeBar.Hide();
            buffStack.HideBlockAll();
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
        if (IsDead) 
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

        int hp = playerStats[(int)PlayerStat.Type.Hp].Value;
        hp -= damage;

        ShowCombatText(
            type: CombatText.Type.Damage,
            text: "-" + damage
        );

        if (hp > 0)
        {
            hpGaugeBar.SetValue(Hp);

            Character.PlayAnimation(PlayerCharacter.AniType.Hit);

            playerStats[(int)PlayerStat.Type.Hp].SetValue(hp);
        }
        else
        {
            OnDeath();
            GameCtrl.Instance.OnGameEnd(Level < StaticValues.CHECKPOINT_LEVEL ? GameResult.Defeat : GameResult.Challenge);
        }

        healTimer = 0f;
    }
    public void OnHeal(int value)
    {
        if (IsDead)
        {
            return;
        }

        int amount = Mathf.Clamp(value, 0, HpMax - Hp);

        if (amount > 0)
        {
            healParticle.Play();
            playerStats[(int)PlayerStat.Type.Hp].IncreaseValue(amount);

            hpGaugeBar.SetValue(Hp);

            if (Hp >= HpMax)
            {
                healTimer = 0f;
                stateToggleGroup.HealGauge.fillAmount = 0f;
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
            buffStack.SetValue(2, $"{(int)BARRIER_DURATION}");
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
            buffStack.SetValue(1, $"{(int)EXP_BOOST_DURATION}");
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
            buffStack.SetValue(3, $"{(int)POWER_UP_DURATION}");
        }
        else
        {
            StartCoroutine(OnPowerUpCor());
        }
    }
    private void ShowCombatText(CombatText.Type type, string text)
    {
        CombatText combatText = UIContainer.Instance.Discharge();
        combatText.Activate(
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
            MoveVec = new Vector3(v.x, v.y, 0f) * speed;
            Character.PlayAnimation(PlayerCharacter.AniType.Move);

            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

            Character.FlipX(MoveVec.x < 0);
        }
        else
        {
            MoveVec = Vector3.zero;
            Character.PlayAnimation(PlayerCharacter.AniType.Idle);
        }
    }
    private IEnumerator OnBarrierCor()
    {
        isBarrier = true;
        barrierParticle.Play();

        ShowCombatText(
            type: CombatText.Type.StateChange,
            text: COMBAT_TEXT_BARRIER
        );

        barrierTimer = 0f;
        while (barrierTimer < BARRIER_DURATION)
        {
            barrierTimer += 1;
            buffStack.SetValue(2, $"{(int)(BARRIER_DURATION - barrierTimer)}");

            yield return WaitForSecond;
        }

        buffStack.HideBlock(2);

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

        expBoostTimer = 0f;
        while (expBoostTimer < EXP_BOOST_DURATION)
        {
            expBoostTimer += 1;
            buffStack.SetValue(1, $"{(int)(EXP_BOOST_DURATION - expBoostTimer)}");

            yield return WaitForSecond;
        }

        buffStack.HideBlock(1);

        isExpBoost = false;
    }
    private IEnumerator OnPowerUpCor()
    {
        isPowerUp = true;

        ShowCombatText(
            type: CombatText.Type.StateChange,
            text: COMBAT_TEXT_POWER_UP
        );

        powerUpTimer = 0f;
        while (powerUpTimer < POWER_UP_DURATION)
        {
            powerUpTimer += 1;
            buffStack.SetValue(3, $"{(int)(POWER_UP_DURATION - powerUpTimer)}");

            yield return WaitForSecond;
        }

        buffStack.HideBlock(3);

        isPowerUp = false;
    }
}
