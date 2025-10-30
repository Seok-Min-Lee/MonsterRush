using UnityEngine;

public class WeaponDFlare : Weapon
{
    [SerializeField] private int knockbackPower = 1;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.OnDamage(power + Player.Instance.Strength);
            enemy.Knockback((enemy.transform.position - transform.position).normalized * knockbackPower);
        }
    }
    public override void Strengthen()
    {
        base.Strengthen();

        knockbackPower++;
    }
    public void Init(int knockbackPower)
    {
        this.knockbackPower = knockbackPower;
    }
}
