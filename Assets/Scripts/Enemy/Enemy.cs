using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State { Live, Shutdown, Dead }
    [SerializeField] protected EnemyCharacter character;
    [SerializeField] protected SpriteRenderer bleedSticker;
    [SerializeField] protected ParticleSystem deathParticle;
    [SerializeField] protected ParticleSystem poisonParticle;
    [SerializeField] protected GaugeBar hpGaugeBar;

    [SerializeField] [Range(0, 7)] protected int groupId;
    [SerializeField] protected SoundKey deathSFX;

    [Header("level Monitor")]
    [SerializeField] protected int hpLevel = 0;
    [SerializeField] protected int powerLevel = 0;
    [SerializeField] protected int speedLevel = 0;

    public EnemyInfo enemyInfo { get; protected set; }
    public BoxCollider2D collider { get; protected set; }
    public Rigidbody2D rigidbody { get; protected set; }
    public Vector3 toPlayer { get; protected set; } = Vector3.zero;
    public State state { get; protected set; } = State.Live;

    protected EnemyContainer pool;

    // 기본 속성
    protected int hp = 1;
    protected int hpMax = 1;
    protected int power;
    protected float speed;

    // 중독
    protected bool isAddict = false;
    protected int addictDamage = 0;
    protected float addictInterval = 0.5f;
    protected float addictTimer = 0f;

    // 출혈
    protected bool isBleed = false;
    protected float bleedPower = 0f;
    protected float bleedTimer = 0f;

    // 둔화
    protected bool isSlow = false;
    protected float slowPower = 0f;

    // 넉백
    protected bool isKnockback = false;
    protected float knockbackTimer = 0f;

    // 슈퍼아머
    protected bool isSuperArmor = false;
    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();

        enemyInfo = DataAssetService.Instance.EnemyDataAsset.Enemies[groupId];
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == State.Live && !StaticValues.isWait && collision.gameObject.CompareTag(TagKeys.PLAYER))
        {
            Player.Instance.OnDamage(power);
            OnDeath();
        }
    }
    public virtual void UpdateTick(float time)
    {
        if (state != State.Live)
        {
            return;
        }

        // 애니메이션
        character.UpdateTick(time);

        // 중독 처리
        if (isAddict)
        {
            if (addictTimer > addictInterval)
            {
                OnDamage(addictDamage);
                addictTimer = 0f;
            }

            addictTimer += time;
        }

        // 출혈 처리
        if (isBleed)
        {
            if (bleedTimer > 2f)
            {
                OffBleed();
                bleedTimer = 0f;
            }

            bleedTimer += time;
        }

        // 이동
        if (Player.Instance != null)
        {
            // 넉백 처리
            if (isKnockback)
            {
                knockbackTimer += time;

                if (knockbackTimer > .2f)
                {
                    knockbackTimer = 0f;
                    isKnockback = false;
                    rigidbody.linearVelocity = Vector2.zero;
                }
            }
            // 평상시 이동 처리
            else
            {
                toPlayer = (Player.Instance.transform.position - transform.position);
                
                transform.position += (Vector3)(toPlayer.normalized * speed * (1 - slowPower) * time);
                collider.enabled = !Player.Instance.IsDead &&
                                    -5 < toPlayer.y && toPlayer.y < 5 &&
                                    -3 < toPlayer.x && toPlayer.x < 3;

                character.FlipX(toPlayer.x > 0);
            }
        }
    }

    public virtual void Spawn(
        EnemyContainer pool, 
        int hpLevel, 
        int powerLevel, 
        int speedLevel, 
        Vector3 position, 
        Quaternion rotation
    )
    {
        state = State.Live;

        this.pool = pool;
        this.hpLevel = hpLevel;
        this.powerLevel = powerLevel;
        this.speedLevel = speedLevel;

        transform.position = position;
        transform.rotation = rotation;

        // 스탯 초기화
        hpMax = (int)(enemyInfo.hp * (1 + 0.1f * hpLevel));
        power = (int)(enemyInfo.power * (1 + 0.1f * powerLevel));
        speed = enemyInfo.speed * (1 + 0.1f * speedLevel);

        hp = hpMax;
        hpGaugeBar.Init(maxValue: hpMax, currentValue: hp, visible: false);
    }
    public virtual void OnShutdown()
    {
        collider.enabled = false;
        rigidbody.linearVelocity = Vector2.zero;

        knockbackTimer = 0f;
        isKnockback = false;

        state = State.Shutdown;
    }
    public virtual void OnDeath()
    {
        if (state == State.Dead)
        {
            return;
        }

        Sequence seq = DOTween.Sequence();

        // death 처리 로직
        seq.AppendCallback(() =>
        {
            AudioManager.Instance.PlaySFX(deathSFX);

            deathParticle.Play();

            state = State.Dead;

            OffAddict();
            OffBleed();
            OffSlow(); 
            OffSuperArmor();

            rigidbody.linearVelocity = Vector2.zero;
            collider.enabled = false;
            character.gameObject.SetActive(false);
            hpGaugeBar.Hide();
        });

        // 파티클 재생 시간
        seq.AppendInterval(1f);

        // 비활성화
        seq.AppendCallback(() =>
        {
            collider.enabled = true;
            gameObject.SetActive(false);
            character.gameObject.SetActive(true);

            pool.Charge(this);
            pool = null;
        });
    }
    public virtual void OnDamage(int damage)
    {
        if (state != State.Live) 
        {
            return;
        }

        AudioManager.Instance.PlaySFX(SoundKey.EnemyHit);
        hp -= (int)(damage * (1 + bleedPower));

        if (hp > 0)
        {
            character.PlayAnimation(EnemyCharacter.AniType.Hit);

            hpGaugeBar.SetValue(hp);
        }
        else
        {
            OnDeath();

            ItemContainer.Instance.Batch(groupId, transform.position);
            Player.Instance.IncreaseKill();
        }
    }
    public virtual void OnAddict(int value)
    {
        if (isSuperArmor)
        {
            return;
        }

        isAddict = true;
        addictDamage = value < 4 ? 1 : 2;
        addictTimer = 0f;
        addictInterval = 0.4f - Mathf.Lerp(0f, 0.3f, (float)value / 8);

        poisonParticle.Play();
    }
    public virtual void OffAddict()
    {
        isAddict = false;
        addictDamage = 0;
        addictTimer = 0f; 
        addictInterval = 0.5f;

        poisonParticle.Stop();
    }
    public virtual void OnBleed(float value)
    {
        if (isSuperArmor)
        {
            return;
        }

        isBleed = true;
        bleedPower = value;
        bleedTimer = 0f;

        bleedSticker.gameObject.SetActive(true);
    }
    public virtual void OffBleed()
    {
        isBleed = false;
        bleedPower = 0f;
        bleedTimer = 0f;

        bleedSticker.gameObject.SetActive(false);
    }
    public virtual void OnSlow(float value)
    {
        if (isSuperArmor)
        {
            return;
        }

        slowPower = 0.25f + Mathf.Lerp(0f, 0.25f, (float)value / 8);

        isSlow = true;

        character.ChangeColor(EnemyCharacter.ColorType.Slow);
    }
    public virtual void OffSlow()
    {
        slowPower = 0f;
        isSlow = false;

        character.ChangeColor(EnemyCharacter.ColorType.Default);
    }
    public virtual void OnSuperArmor()
    {
        isSuperArmor = true;
    }
    public virtual void OffSuperArmor()
    {
        isSuperArmor = false;
    }
    public virtual void OnKnockback(Vector3 direction)
    {
        if (isSuperArmor)
        {
            return;
        }

        if (state == State.Live &&
            direction != Vector3.zero &&
            rigidbody.linearVelocity == Vector2.zero)
        {
            rigidbody.AddForce(direction, ForceMode2D.Impulse);
            knockbackTimer = 0f;
        }

        isKnockback = true;
    }
}
