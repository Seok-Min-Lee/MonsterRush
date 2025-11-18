using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponB : Weapon
{
    [SerializeField] private int speed;
    [SerializeField] private float bleedRatio = 0.1f;
    [SerializeField] private float bleedPower = 0.5f;
    [SerializeField] private bool isPenetrate;

    private Rigidbody2D rigidbody;
    private WeaponContainerB container;
    private float timer = 0f;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (timer > 0.5f)
        {
            OnReload();
        }

        timer += Time.deltaTime;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.OnDamage(power + Player.Instance.Strength);

            if (Random.Range(0f, 1f) < bleedRatio)
            {
                enemy.OnBleed(bleedPower);
            }

            if (!isPenetrate)
            {
                OnReload();
            }
        }
    }
    public void OnShot(WeaponContainerB container, float bleedRatio, bool isPenetrate, Vector3 position, Quaternion rotation, Vector3 direction)
    {
        gameObject.SetActive(true);

        this.container = container;
        this.bleedRatio = bleedRatio;
        this.isPenetrate = isPenetrate;
        transform.position = position;
        transform.rotation = rotation;

        rigidbody.linearVelocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;
        rigidbody.AddForce(direction * speed, ForceMode2D.Impulse);

        timer = 0f;
    }
    public void OnReload()
    {
        gameObject.SetActive(false);
        container.Reload(this);
    }
}
