using UnityEngine;

public class WeaponA : Weapon
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.OnDamage(power + Player.Instance.Strength);
            enemy.Knockback(Vector3.zero);
        }
    }
    public override void Strengthen()
    {
        power += 3;
    }
}
