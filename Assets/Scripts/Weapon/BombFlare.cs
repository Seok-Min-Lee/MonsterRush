using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombFlare : Weapon
{
    private float SCALE_UP_RATIO = 1.75f;
    [SerializeField] private float radius = 0;

    private bool isScaleUp = false;
    private int knockbackLevel = 0;

    private ParticleSystem particle;
    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.OnDamage(power + Player.Instance.Strength);

            Vector3 knockbackForce = (enemy.transform.position - transform.position).normalized * (0.8f + knockbackLevel * 0.4f);
            enemy.OnKnockback(knockbackForce);
        }
    }
    public void OnExplosion()
    {
        particle.Play();

        List<Enemy> enemies = EnemyContainer.Instance.GetActiveEnemyAll();

        float r = isScaleUp ? radius * radius * SCALE_UP_RATIO * SCALE_UP_RATIO : radius * radius;
        for (int i = 0; i < enemies.Count; i++)
        {
            float sqrMagnitude = Vector3.SqrMagnitude(transform.position - enemies[i].transform.position);

            if (sqrMagnitude < r)
            {
                Enemy enemy = enemies[i];
                enemy.OnDamage(power + Player.Instance.Strength);

                Vector3 knockbackForce = (enemy.transform.position - transform.position).normalized * (0.8f + knockbackLevel * 0.4f);
                enemy.OnKnockback(knockbackForce);
            }
        }
    }
    public void Init(bool isScaleUp,  int knockbackLevel)
    {
        transform.localScale = Vector3.one * (isScaleUp ? SCALE_UP_RATIO : 1f);
        this.isScaleUp = isScaleUp;
        this.knockbackLevel = knockbackLevel;
    }
    private void OnDrawGizmosSelected()
    {
        if (Player.Instance == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, isScaleUp ? radius * SCALE_UP_RATIO : radius);
    }
}
