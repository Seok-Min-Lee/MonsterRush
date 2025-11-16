using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject character;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private ParticleSystem poisonParticle;
    [SerializeField] private Color slowColor;

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

    private Transform target;
    private EnemyPool pool;

    private BoxCollider2D collider;
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float addictTimer = 0f;
    private float bleedTimer = 0f;
    private float knockbackTimer = 0f;

    public bool isDead { get; private set; } = false;
    private bool isKnockback = false;
    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();

        spriteRenderer = character.GetComponent<SpriteRenderer>();
        animator = character.GetComponent<Animator>();
    }
    private void Update()
    {
        if (isAddict)
        {
            if (addictTimer > addictInterval)
            {
                OnDamage(addictDamage);
                addictTimer = 0f;
            }

            addictTimer += Time.deltaTime;
        }

        if (isBleed)
        {
            if (bleedTimer > 2f)
            {
                OffBleed();
                bleedTimer = 0f;
            }

            bleedTimer += Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        Move();
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
        target = Player.Instance.transform;

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
            particle.Play();

            isDead = true;
            target = null;

            OffAddict();
            OffBleed();
            OffSlow();

            rigidbody.linearVelocity = Vector2.zero;
            collider.enabled = false;
            character.SetActive(false);
            canvasGO.SetActive(false);
        });

        // 파티클 재생 시간
        seq.AppendInterval(1f);

        // 비활성화
        seq.AppendCallback(() =>
        {
            collider.enabled = true;
            gameObject.SetActive(false);
            character.SetActive(true);
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
            animator.SetTrigger("doHit");

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
        addictDamage = 1;
        addictTimer = 0f;
        addictInterval = 0.33333f - value * 0.0291625f; // 0.33333 ~ 0.1
        
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
        slowPower = 0.1f + value * 0.05f;
        isSlow = true;

        spriteRenderer.color = slowColor;
    }
    public virtual void OffSlow()
    {
        slowPower = 0f;
        isSlow = false;

        spriteRenderer.color = Color.white;
    }
    protected virtual void Move()
    {
        if (target == null) 
        {
            return;
        }

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

            Vector2 dir = (target.position - transform.position).normalized;
            transform.position += (Vector3)(dir * speed * (1 - slowPower) * Time.deltaTime);
            spriteRenderer.flipX = dir.x > 0;
        }
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
