using UnityEngine;

public class WeaponA : Weapon
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Sprite[] sprites;

    private int spriteIndex = 0;

    private bool isKnockback;

    [Header("Debug")]
    [SerializeField] private int powerLevel = 0;
    public void UpdateTick()
    {
        renderer.sprite = sprites[spriteIndex++ % sprites.Length];
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.OnDamage(power + powerLevel * 2 + Player.Instance.Strength);

            if (isKnockback)
            {
                enemy.OnKnockback(Vector3.zero);
            }
        }
    }
    public void PowerUp(int level)
    {
        powerLevel = level;

        renderer.color = new Color(1f, 1f - powerLevel * 0.0625f, 1f - powerLevel * 0.0625f, 1f);
    }
    public void ActivateKnockback()
    {
        isKnockback = true;
    }
}
