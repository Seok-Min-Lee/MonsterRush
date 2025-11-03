using DG.Tweening;
using UnityEngine;

public class WeaponC : Weapon
{
    [SerializeField] private Transform maskTransform;
    [SerializeField] private SpriteRenderer textureRenderer;
    [SerializeField] private float slowPower = 0f;

    public SpriteRenderer TextureRenderer => textureRenderer;
    private CircleCollider2D collider;

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
    }
    private void Start()
    {
        Vector3 maskScale = Vector3.one * 2f;
        float collierRadius = 1f;
        Vector2 textureSize = Vector2.one * 2f;

        maskTransform.localScale = Vector3.zero;
        collider.radius = 0f;
        textureRenderer.size = Vector2.zero;

        maskTransform.DOScale(maskScale, 0.5f);
        DOTween.To(() => collider.radius, x => collider.radius = x, collierRadius, 0.5f);
        DOTween.To(() => textureRenderer.size, x => textureRenderer.size = x, textureSize, 0.5f);
    }
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, .5f);
    }
    public void Expand()
    {
        Vector3 maskScale = maskTransform.localScale + Vector3.one * 0.25f;
        float collierRadius = collider.radius + 0.125f;
        Vector2 textureSize = textureRenderer.size + Vector2.one * 0.25f;

        maskTransform.DOScale(maskScale, 0.5f);
        DOTween.To(() => collider.radius, x => collider.radius = x, collierRadius, 0.5f);
        DOTween.To(() => textureRenderer.size, x => textureRenderer.size = x, textureSize, 0.5f);
    }
    public override void Strengthen()
    {
        slowPower += 0.05f;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.OnAddict(power);

            if (slowPower > 0f)
            {
                enemy.OnSlow(slowPower);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            enemy.OffAddict();

            if (slowPower > 0f)
            {
                enemy.OffSlow();
            }
        }
    }
}
