using UnityEngine;

public class WeaponA : Weapon
{
    private bool isKnockback;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.OnDamage(power + Player.Instance.Strength);

            if (isKnockback)
            {
                enemy.OnKnockback(Vector3.zero);
            }
        }
    }
    public void PowerUp()
    {
        power += 2;
    }
    public void ActivateKnockback()
    {
        isKnockback = true;
    }
}
