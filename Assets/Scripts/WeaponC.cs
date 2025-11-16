using DG.Tweening;
using UnityEngine;

public class WeaponC : Weapon
{
    [SerializeField] private Transform maskTransform;
    [SerializeField] private SpriteRenderer textureRenderer;
    private Color poisonColor = new Color(0.7529f, 0, 1f, 1f);
    private Color slowcolor = new Color(0f, 0.5f, 1f, 1f);

    private bool isModePosion = true;

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
    }
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, .5f);
    }
    private Collider2D[] detectBuffer = new Collider2D[200];
    public void SwitchMode(bool value)
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

            if (isModePosion)
            {
                enemy.OnAddict(power);
                enemy.OffSlow();
            }
            else
            {
                enemy.OnSlow(power);
                enemy.OffAddict();
            }
        }
    }
    public void Init(bool value)
    {
        isModePosion = value;
        textureRenderer.color = isModePosion ? poisonColor : slowcolor;
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
    public override void Strengthen()
    {
        power++;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (isModePosion)
            {
                enemy.OnAddict(power);
                enemy.OffSlow();
            }
            else
            {
                enemy.OnSlow(power);
                enemy.OffAddict();
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
