using DG.Tweening;
using UnityEngine;

public class WeaponC : Weapon
{
    [SerializeField] private Transform maskTransform;
    [SerializeField] private SpriteRenderer textureRenderer;
    [SerializeField] private float slowPower = 0f;

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
        Vector3 maskScale = Vector3.one * 2f;
        Vector2 textureSize = Vector2.one * 2f;
        float collierRadius = 1f;

        // 葛记 利侩
        maskTransform.DOScale(maskScale, 0.5f);
        DOTween.To(() => textureRenderer.size, x => textureRenderer.size = x, textureSize, 0.5f);
        DOTween.To(() => collider.radius, x => collider.radius = x, collierRadius, 0.5f);
    }
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, .5f);
    }
    public void SwitchVisibility(bool value)
    {
        textureRenderer.enabled = value;
    }
    public void Expand()
    {
        // Target Value
        Vector3 maskScale = maskTransform.localScale + Vector3.one * 0.25f;
        Vector2 textureSize = textureRenderer.size + Vector2.one * 0.25f;
        float collierRadius = collider.radius + 0.125f;

        // 葛记 利侩
        maskTransform.DOScale(maskScale, 0.5f);
        DOTween.To(() => textureRenderer.size, x => textureRenderer.size = x, textureSize, 0.5f);
        DOTween.To(() => collider.radius, x => collider.radius = x, collierRadius, 0.5f);
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
            enemy.OnSlow(slowPower);
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
}
