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
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, .5f);
    }
    public void Expand()
    {
        collider.radius += 0.125f;
        maskTransform.localScale += Vector3.one * 0.25f;
        textureRenderer.size += Vector2.one * 0.25f;
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
