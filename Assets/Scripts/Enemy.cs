using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyCharacter character;
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private ParticleSystem poisonParticle;

    [SerializeField] [Range(0, 7)] private int itemIndex;

    [Header("Current Value")]
    [SerializeField] protected int hp = 1;
    [SerializeField] private int hpMax = 1;
    [SerializeField] protected int power;
    [SerializeField] protected float speed;
    [SerializeField] protected float addictInterval = 0.5f;
    [SerializeField] protected int addictDamage = 0;
    [SerializeField] protected bool isAddict = false;
    [SerializeField] protected float bleedPower = 0f;
    [SerializeField] protected bool isBleed = false;
    [SerializeField] protected float slowPower = 0f;
    [SerializeField] protected bool isSlow = false;

    [Header("Default Value")]
    [SerializeField] private int hpDefault = 1;
    [SerializeField] private int powerDefault = 1;
    [SerializeField] private float speedDefault = 1f;

    [Header("level Value")]
    [SerializeField] private int hpLevel = 0;
    [SerializeField] private int powerLevel = 0;
    [SerializeField] private int speedLevel = 0;

    [Header("UI")]
    [SerializeField] private GameObject canvasGO;
    [SerializeField] private Image hpGuage;
    [SerializeField] private Image bleedSticker;

    private EnemyPool pool;

    private BoxCollider2D collider;
    private Rigidbody2D rigidbody;

    private float addictTimer = 0f;
    private float bleedTimer = 0f;
    private float knockbackTimer = 0f;

    public bool isDead { get; private set; } = false;
    private bool isKnockback = false;
    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
    }
    public void UpdateTick(float time)
    {
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

        // animation
        character.UpdateTick(time);

        // move
        if (Player.Instance != null && !isDead)
        {
            if (isKnockback)
            {
                knockbackTimer += Time.deltaTime;

                if (knockbackTimer > .2f)
                {
                    knockbackTimer = 0f;
                    isKnockback = false;
                    rigidbody.linearVelocity = Vector2.zero;
                }
            }
            else
            {
                if (rigidbody.linearVelocity != Vector2.zero)
                {
                    rigidbody.linearVelocity = Vector2.zero;
                }

                Vector2 dir = (Player.Instance.transform.position - transform.position);
                transform.position += (Vector3)(dir.normalized * speed * (1 - slowPower) * Time.deltaTime);
                collider.enabled = !Player.Instance.IsDead && dir.sqrMagnitude < 25;
                character.FlipX(dir.x > 0);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDead && collision.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX(SoundKey.PlayerHit);
            Player.Instance.OnDamage(power);
            OnDeath();
        }
    }

    public virtual void Spawn(EnemyPool pool, int hpLevel, int powerLevel, int speedLevel, Vector3 position, Quaternion rotation)
    {
        //
        isDead = false;

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
        hpGuage.fillAmount = 1f;

        canvasGO.SetActive(false);
    }
    public virtual void OnDeath()
    {
        Sequence seq = DOTween.Sequence();

        // death 처리 로직
        seq.AppendCallback(() =>
        {
            deathParticle.Play();

            isDead = true;

            OffAddict();
            OffBleed();
            OffSlow();

            rigidbody.linearVelocity = Vector2.zero;
            collider.enabled = false;
            character.gameObject.SetActive(false);
            canvasGO.SetActive(false);
        });

        // 파티클 재생 시간
        seq.AppendInterval(1f);

        // 비활성화
        seq.AppendCallback(() =>
        {
            collider.enabled = true;
            gameObject.SetActive(false);
            character.gameObject.SetActive(true);
            canvasGO.SetActive(true);

            pool.Charge(this);
            pool = null;
        });
    }
    public virtual void OnDamage(int damage)
    {
        if (isDead) return;

        AudioManager.Instance.PlaySFX(SoundKey.EnemyHit);
        hp -= (int)(damage * (1 + bleedPower));

        if (hp > 0)
        {
            character.PlayAnimation(EnemyCharacter.AniType.Hit);

            canvasGO.SetActive(true);
            hpGuage.fillAmount = (float)hp / (float)hpMax;
        }
        else
        {
            OnDeath();

            ItemContainer.Instance.Batch(itemIndex, transform.position);
            Player.Instance.KillEnemy();
        }
    }
    public virtual void OnAddict(int value)
    {
        isAddict = true;
        addictDamage = value < 4 ? 1 : 2;
        addictTimer = 0f;
        addictInterval = 0.5f - value * 0.0375f; // 0.5 ~ 0.2
        
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
        slowPower = 0.3f + value * 0.025f;
        isSlow = true;

        character.ChangeColor(EnemyCharacter.ColorType.Slow);
    }
    public virtual void OffSlow()
    {
        slowPower = 0f;
        isSlow = false;

        character.ChangeColor(EnemyCharacter.ColorType.Default);
    }
    public virtual void OnKnockback(Vector3 direction)
    {
        if (!isDead &&
            direction != Vector3.zero &&
            rigidbody.linearVelocity == Vector2.zero)
        {
            rigidbody.AddForce(direction, ForceMode2D.Impulse);
            knockbackTimer = 0f;
        }

        isKnockback = true;
    }

}
