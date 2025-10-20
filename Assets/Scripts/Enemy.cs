using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Current Value")]
    [SerializeField] protected int hp = 1;
    [SerializeField] protected bool isAddict = false;
    [SerializeField] protected int addictDamage = 0;

    [Header("Init Value")]
    [SerializeField] private int hpMax = 1;

    [Header("Constant Value")]
    [SerializeField] private float speed = 1f;

    [Header("UI")]
    [SerializeField] GameObject canvasGO;
    [SerializeField] private Image guage;

    private Transform player;
    private EnemyContainer container;
    private Rigidbody2D rigidbody;

    private float addictTimer = 0f;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
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

    public virtual void Spawn(Vector3 position, Quaternion rotation, EnemyContainer container)
    {
        hp = hpMax;
        guage.fillAmount = 1f;
        canvasGO.SetActive(false);

        transform.position = position;
        transform.rotation = rotation;

        player = Player.Instance.transform;
        this.container = container;
    }
    public virtual void Die()
    {
        OffAddict();

        gameObject.SetActive(false);
        container.Charge(this);

        ItemContainer.Instance.Batch(transform.position);

        player = null;
        container = null;
    }
    public virtual void OnDamage(int damage)
    {
        hp -= damage;

        if (hp < 0)
        {
            Die();
        }
        else
        {
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
        if (player == null)
        {
            return;
        }

        if (rigidbody.linearVelocity != Vector2.zero)
        {
            rigidbody.linearVelocity = Vector2.zero;
        }

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }
}
