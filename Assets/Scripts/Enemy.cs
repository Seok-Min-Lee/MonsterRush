using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public enum State { Live, Shutdown, Dead }
    [SerializeField] protected EnemyCharacter character;
    [SerializeField] protected SpriteRenderer bleedSticker;
    [SerializeField] protected ParticleSystem deathParticle;
    [SerializeField] protected ParticleSystem poisonParticle;
    [SerializeField] protected GameObject hpBar;
    [SerializeField] protected Transform hpGuage;

    [SerializeField] [Range(0, 7)] protected int itemIndex;
    [SerializeField] protected SoundKey deathSFX;

    [Header("Current Value")]
    [SerializeField] protected int hp = 1;
    [SerializeField] protected int hpMax = 1;
    [SerializeField] protected int power;
    [SerializeField] protected float speed;
    [SerializeField] protected bool isAddict = false;
    [SerializeField] protected bool isBleed = false;
    [SerializeField] protected bool isSlow = false;
    protected int addictDamage = 0;
    protected float addictInterval = 0.5f;
    protected float bleedPower = 0f;
    protected float slowPower = 0f;

    [Header("Default Value")]
    [SerializeField] protected int hpDefault = 1;
    [SerializeField] protected int powerDefault = 1;
    [SerializeField] protected float speedDefault = 1f;

    [Header("level Value")]
    [SerializeField] protected int hpLevel = 0;
    [SerializeField] protected int powerLevel = 0;
    [SerializeField] protected int speedLevel = 0;

    protected EnemyPool pool;
    public BoxCollider2D collider { get; private set; }
    public Rigidbody2D rigidbody { get; private set; }
    public Vector3 toPlayer { get; private set; } = Vector3.zero;

    protected float addictTimer = 0f;
    protected float bleedTimer = 0f;
    protected float knockbackTimer = 0f;

    public State state = State.Live;
    protected bool isKnockback = false;
    protected bool isSuperArmor = false;
    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == State.Live && !StaticValues.isWait && collision.gameObject.CompareTag("Player"))
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

        // animation
        character.UpdateTick(time);

        // addict
        if (isAddict)
        {
            if (addictTimer > addictInterval)
            {
                OnDamage(addictDamage);
                addictTimer = 0f;
            }

            addictTimer += time;
        }

        // bleed
        if (isBleed)
        {
            if (bleedTimer > 2f)
            {
                OffBleed();
                bleedTimer = 0f;
            }

            bleedTimer += time;
        }

        // move
        if (Player.Instance != null)
        {
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

    public virtual void Spawn(EnemyPool pool, int hpLevel, int powerLevel, int speedLevel, Vector3 position, Quaternion rotation)
    {
        //
        state = State.Live;

        this.pool = pool;
        this.hpLevel = hpLevel;
        this.powerLevel = powerLevel;
        this.speedLevel = speedLevel;

        transform.position = position;
        transform.rotation = rotation;

        //
        hpMax = (int)(hpDefault * (1 + 0.1f * hpLevel));
        power = (int)(powerDefault * (1 + 0.1f * powerLevel));
        speed = speedDefault * (1 + 0.1f * speedLevel);

        hp = hpMax;
        hpGuage.localScale = Vector3.one;
        hpBar.SetActive(false);
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
            hpBar.SetActive(false);
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

            hpBar.SetActive(true);
            hpGuage.localScale = new Vector3((float)hp / hpMax, 1, 1);
        }
        else
        {
            OnDeath();

            ItemContainer.Instance.Batch(itemIndex, transform.position);
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
