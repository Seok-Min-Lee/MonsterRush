using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Current Value")]
    [SerializeField] protected int hp = 1;
    [SerializeField] protected int power;
    [SerializeField] protected bool isAddict = false;
    [SerializeField] protected int addictDamage = 0;

    [Header("Init Value")]
    [SerializeField] private int hpMax = 1;

    [Header("Constant Value")]
    [SerializeField] private float speed = 1f;

    [Header("UI")]
    [SerializeField] GameObject canvasGO;
    [SerializeField] private Image guage;

    private Transform target;
    private EnemyContainer container;
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float addictTimer = 0f;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (isAddict)
        {
            if (addictTimer > 1f)
            {
                OnDamage(addictDamage);
                addictTimer = 0f;
            }

            addictTimer += Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.OnDamage(power);
            Die();
        }
    }

    public virtual void Spawn(Vector3 position, Quaternion rotation, EnemyContainer container)
    {
        hp = hpMax;
        guage.fillAmount = 1f;
        canvasGO.SetActive(false);

        transform.position = position;
        transform.rotation = rotation;

        target = Player.Instance.transform;
        this.container = container;
    }
    public virtual void Die()
    {
        OffAddict();

        gameObject.SetActive(false);
        container.Charge(this);

        target = null;
        container = null;
    }
    public virtual void OnDamage(int damage)
    {
        hp -= damage;

        if (hp < 0)
        {
            Die();

            if (ItemContainer.Instance != null)
            {
                ItemContainer.Instance.Batch(transform.position);
            }

            if (Player.Instance != null)
            {
                Player.Instance.KillEnemy();
            }
        }
        else
        {
            animator.SetTrigger("onDamage");

            canvasGO.SetActive(true);
            guage.fillAmount = (float)hp / (float)hpMax;
        }
    }
    public virtual void OnAddict(int value)
    {
        isAddict = true;
        addictDamage = value;
        addictTimer = 0f;
    }
    public virtual void OffAddict()
    {
        addictTimer = 0f;
        addictDamage = 0;
        isAddict = false;
    }
    protected virtual void Move()
    {
        if (target == null)
        {
            return;
        }

        if (rigidbody.linearVelocity != Vector2.zero)
        {
            rigidbody.linearVelocity = Vector2.zero;
        }

        Vector2 dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
        spriteRenderer.flipX = dir.x > 0;
    }
}
