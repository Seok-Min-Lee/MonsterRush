using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Weapon
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Sprite[] sprites;

    private int speed = 10;
    private int bleedLevel = 0;
    private bool isPenetrate = false;

    private Rigidbody2D rigidbody;
    private KnifeLauncher container;

    private bool isUsed = false;
    private float bleedRatio = 0.1f;
    private float bleedPower = 0.5f;
    private float timer = 0f;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (timer > 1f)
        {
            OnReload();
        }

        timer += Time.deltaTime;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (isUsed)
        {
            return;
        }

        else if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.OnDamage(power + Player.Instance.Strength);

            if (Random.Range(0f, 1f) < bleedRatio)
            {
                enemy.OnBleed(bleedPower);
            }

            if (!isPenetrate)
            {
                timer = 1f;
                isUsed = true;
            }
        }
        else if (collision.CompareTag("ItemBox"))
        {
            collision.GetComponent<ItemBox>().onHit();

            if (!isPenetrate)
            {
                timer = 1f;
                isUsed = true;
            }
        }
    }
    public void OnShot(KnifeLauncher container, int bleedLevel, bool isPenetrate, Vector3 position, Quaternion rotation, Vector3 direction)
    {
        gameObject.SetActive(true);

        this.container = container;
        this.bleedLevel = bleedLevel;
        this.isPenetrate = isPenetrate;

        bleedRatio = 0.1f + 0.05f * bleedLevel;
        renderer.sprite = isPenetrate ? sprites[1] : sprites[0];

        transform.position = position;
        transform.rotation = rotation;

        rigidbody.linearVelocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;
        rigidbody.AddForce(direction * speed, ForceMode2D.Impulse);

        timer = 0f;
        isUsed = false;
    }
    public void OnReload()
    {
        rigidbody.linearVelocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;

        gameObject.SetActive(false);
        container.Charge(this);
    }
}
