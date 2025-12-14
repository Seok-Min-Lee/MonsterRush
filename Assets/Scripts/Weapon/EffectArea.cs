using DG.Tweening;
using UnityEngine;

public class EffectArea : Weapon
{
    public enum State { Poison, Slow, Both }

    [SerializeField] private Transform maskTransform;
    [SerializeField] private SpriteRenderer textureRenderer;

    private State state = State.Poison;
    private int areaLevel = 0;
    private int effectLevel = 0;

    private Color PoisonColor => new Color(0.7529f, 0, 1f, 0.5f + effectLevel * 0.0625f);
    private Color SlowColor => new Color(0f, 0.7529f, 1f, 0.5f + effectLevel * 0.0625f);
    private Color BothColor => new Color(0.37645f, 0.37645f, 1f, 0.5f + effectLevel * 0.0625f);


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
    }
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, .5f);
    }
    private Collider2D[] detectBuffer = new Collider2D[200];
    public void SwitchMode(State value)
    {
        Refresh(value);

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
    public void Refresh(State value)
    {
        state = value;

        Color color = Color.white;
        switch (state)
        {
            case State.Poison:
                color = PoisonColor;
                break;
            case State.Slow:
                color = SlowColor;
                break;
            case State.Both:
                color = BothColor;
                break;
        }

        textureRenderer.DOColor(color, 0.5f);
    }
    public void Expand(int level)
    {
        areaLevel = level;

        // Target Value
        Vector3 maskScale = Vector3.one * (2f + level * 0.25f);
        Vector2 textureSize = Vector2.one * (2f + level * 0.25f);
        float collierRadius = 1f + (level * 0.125f);

        // 모션 적용
        maskTransform.DOScale(maskScale, 0.5f);
        DOTween.To(() => textureRenderer.size, x => textureRenderer.size = x, textureSize, 0.5f);
        DOTween.To(() => collider.radius, x => collider.radius = x, collierRadius, 0.5f);
    }
    public void PowerUp(int level)
    {
        power = level;
        effectLevel = level;
        Refresh(state);
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
