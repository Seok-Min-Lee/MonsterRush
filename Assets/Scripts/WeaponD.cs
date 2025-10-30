using System.Collections;
using UnityEngine;

public class WeaponD : Weapon
{
    [SerializeField] private WeaponDFlare flare;

    private Rigidbody2D rigidbody;
    private WeaponContainerD container;
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Explosion();
        }
    }
    public void Init(WeaponContainerD container, int knockbackPower)
    {
        this.container = container;
        flare.Init(knockbackPower);
    }
    
    public void Shot(Vector3 position, Quaternion rotation, Vector3 direction)
    {
        gameObject.SetActive(true);

        transform.position = position;
        transform.rotation = rotation;
        rigidbody.AddForce(direction * Random.Range(8f, 16f), ForceMode2D.Impulse);
        rigidbody.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
    }
    public void Explosion()
    {
        AudioManager.Instance.PlaySFX(SoundKey.WeaponDExplosion);

        rigidbody.linearVelocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;
        rigidbody.gravityScale = 0f;

        flare.gameObject.SetActive(true);
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            yield return new WaitForSeconds(.15f);
            OnReload();
        }
    }
    public void OnReload()
    {
        timer = 0f;

        rigidbody.linearVelocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;
        rigidbody.gravityScale = 5f;

        gameObject.SetActive(false);
        container.Reload(this);

        flare.gameObject.SetActive(false);
    }
    public override void Strengthen()
    {
        flare.Strengthen();
    }
}
