using System.Collections;
using UnityEngine;

public class WeaponDFlare : Weapon
{
    [SerializeField] private int knockbackLevel = 1;

    private ParticleSystem particle;
    private BoxCollider2D collider;
    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        collider = GetComponent<BoxCollider2D>();

        collider.enabled = false;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.OnDamage(power + Player.Instance.Strength);

            Vector3 knockbackForce = (enemy.transform.position - transform.position).normalized * (knockbackLevel * 0.5f);
            enemy.OnKnockback(knockbackForce);
        }
    }
    public void OnExplosion()
    {
        particle.Play();
        collider.enabled = true;
    }
    public void OffExplosion()
    {
        collider.enabled = false;
        particle.Stop();
    }
    public void Init(int knockbackLevel)
    {
        this.knockbackLevel = knockbackLevel;
    }
}
