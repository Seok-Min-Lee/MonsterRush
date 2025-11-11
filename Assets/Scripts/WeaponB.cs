using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponB : Weapon
{
    [SerializeField] private int speed;
    [SerializeField] private float bleedRatio = 0.1f;
    [SerializeField] private float bleedPower = 0.5f;

    public SpriteRenderer spriteRenderer { get; private set; }

    private Rigidbody2D rigidbody;
    private WeaponContainerB container;
    private float timer = 0f;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (timer > 1f)
        {
            OnReload();

            timer = 0f;
        }

        timer += Time.deltaTime;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.OnDamage(power + Player.Instance.Strength);
            enemy.OnKnockback(Vector3.zero);

            if (Random.Range(0f, 1f) < bleedRatio)
            {
                enemy.OnBleed(bleedPower);
            }
        }
    }
    public void OnShot(WeaponContainerB container, float bleedRatio, bool flipX, Vector3 position, Quaternion rotation, Vector3 direction)
    {
        gameObject.SetActive(true);

        this.container = container;
        this.bleedRatio = bleedRatio;
        spriteRenderer.flipX = flipX;
        transform.position = position;
        transform.rotation = rotation;

        rigidbody.AddForce(direction * speed, ForceMode2D.Impulse);
    }
    public void OnReload()
    {
        rigidbody.angularDamping = 0f;
        rigidbody.angularVelocity = 0f;

        gameObject.SetActive(false);
        container.Reload(this);
    }
}
