using UnityEngine;

public class WeaponC : Weapon
{
    [SerializeField] private Transform maskTransform;
    [SerializeField] private SpriteRenderer textureRenderer;

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
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().OnAddict(power);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().OffAddict();
        }
    }
}
