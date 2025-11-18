using DG.Tweening;
using UnityEngine;

public class WeaponC : Weapon
{
    public enum State { Poison, Slow, Both }
    [SerializeField] private Transform maskTransform;
    [SerializeField] private SpriteRenderer textureRenderer;
    private Color poisonColor = new Color(0.7529f, 0, 1f, 1f);
    private Color slowcolor = new Color(0f, 0.7529f, 1f, 1f);
    private Color bothColor = new Color(0.37645f, 0.37645f, 1f, 1f);

    private State state = State.Poison;

    private CircleCollider2D collider;

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
    }
    private void Start()
    {
        // Init Value
        maskTransform.localScale = Vector3.zero;
        textureRenderer.size = Vector2.zero;
        collider.radius = 0f;

        // Target Value
        Vector3 maskScale = Vector3.one * 2.25f;
        Vector2 textureSize = Vector2.one * 2.25f;
        float collierRadius = 1.125f;

        // 모션 적용
        maskTransform.DOScale(maskScale, 0.5f);
        DOTween.To(() => textureRenderer.size, x => textureRenderer.size = x, textureSize, 0.5f);
        DOTween.To(() => collider.radius, x => collider.radius = x, collierRadius, 0.5f);

        state = State.Poison;
        textureRenderer.color = poisonColor;
    }
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, .5f);
    }
    private Collider2D[] detectBuffer = new Collider2D[200];
    public void SwitchMode(State value)
    {
        Init(value);

        // 주변 적 탐색 후 효과 적용
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = LayerMask.GetMask("Enemy");

        int detectCount = Physics2D.OverlapCircle(
            point: Player.Instance.transform.position,
            radius: collider.radius,
            contactFilter: contactFilter,
            results: detectBuffer
        );

        for (int i = 0; i < detectCount; i++)
        {
            Enemy enemy = detectBuffer[i].gameObject.GetComponent<Enemy>();

            switch (state)
            {
                case State.Poison:
                    enemy.OnAddict(power);
                    enemy.OffSlow();
                    break;
                case State.Slow:
                    enemy.OnSlow(power);
                    enemy.OffAddict();
                    break;
                case State.Both:
                    enemy.OnAddict(power);
                    enemy.OnSlow(power);
                    break;
            }
        }
    }
    public void Init(State value)
    {
        state = value;

        switch (state)
        {
            case State.Poison:
                textureRenderer.color = poisonColor;
                break;
            case State.Slow:
                textureRenderer.color = slowcolor;
                break;
            case State.Both:
                textureRenderer.color = bothColor;
                break;
        }
    }
    public void Expand()
    {
        // Target Value
        Vector3 maskScale = maskTransform.localScale + Vector3.one * 0.25f;
        Vector2 textureSize = textureRenderer.size + Vector2.one * 0.25f;
        float collierRadius = collider.radius + 0.125f;

        // 모션 적용
        maskTransform.DOScale(maskScale, 0.5f);
        DOTween.To(() => textureRenderer.size, x => textureRenderer.size = x, textureSize, 0.5f);
        DOTween.To(() => collider.radius, x => collider.radius = x, collierRadius, 0.5f);
    }
    public void PowerUp()
    {
        power++;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            switch (state)
            {
                case State.Poison:
                    enemy.OnAddict(power);
                    enemy.OffSlow();
                    break;
                case State.Slow:
                    enemy.OnSlow(power);
                    enemy.OffAddict();
                    break;
                case State.Both:
                    enemy.OnAddict(power);
                    enemy.OnSlow(power);
                    break;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.OffAddict();
            enemy.OffSlow();
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (Player.Instance == null)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Player.Instance.transform.position, collider.radius);
    }
}
