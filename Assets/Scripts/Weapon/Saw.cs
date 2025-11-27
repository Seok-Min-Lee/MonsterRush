using DG.Tweening;
using UnityEngine;

public class Saw : Weapon
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Sprite[] sprites;

    private bool isKnockback = false;
    private int powerLevel = 0;

    private int spriteIndex = 0;
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
    }
    public void ActivateKnockback()
    {
        isKnockback = true;
        renderer.DOColor(new Color(1f, 0.5f, 0.5f, 1f), 0.5f);
    }
}
