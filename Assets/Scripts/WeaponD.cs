using DG.Tweening;
using System.Collections;
using UnityEngine;

public class WeaponD : Weapon
{
    [SerializeField] private WeaponDFlare flare;

    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private WeaponContainerD container;
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
        }

        timer += Time.deltaTime;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            OnExplosion();
        }
    }
    
    public void OnShot(WeaponContainerD container, int knockbackLevel, float explosionScale, Vector3 position, Quaternion rotation, Vector3 direction)
    {
        gameObject.SetActive(true);

        this.container = container;
        flare.Init(knockbackLevel);
        transform.position = position;
        transform.rotation = rotation;
        flare.transform.localScale = Vector3.one * explosionScale;

        Vector3 force = direction * Random.Range(8f, 16f);
        float torque = Random.Range(-10f, 10f);

        rigidbody.AddForce(force, ForceMode2D.Impulse);
        rigidbody.AddTorque(torque, ForceMode2D.Impulse);
    }
    public void OnExplosion()
    {
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            AudioManager.Instance.PlaySFX(SoundKey.WeaponDExplosion);

            rigidbody.linearVelocity = Vector2.zero;
            rigidbody.angularVelocity = 0f;
            rigidbody.gravityScale = 0f;

            spriteRenderer.enabled = false;

            flare.OnExplosion();

            yield return new WaitForSeconds(.15f);

            flare.OffExplosion();
            OnReload();
        }

        // DoTween을 쓰면 OnReload에서 Rigidbody 초기화 안 되는 오류 발생
        //Sequence seq = DOTween.Sequence();
        //seq.AppendCallback(() =>
        //{
        //    AudioManager.Instance.PlaySFX(SoundKey.WeaponDExplosion);

        //    rigidbody.linearVelocity = Vector2.zero;
        //    rigidbody.angularVelocity = 0f;
        //    rigidbody.gravityScale = 0f;

        //    spriteRenderer.enabled = false;

        //    flare.OnExplosion();
        //});
        //seq.AppendInterval(0.15f);
        //seq.AppendCallback(() =>
        //{
        //    flare.OffExplosion();
        //    OnReload();
        //});
    }
    public void OnReload()
    {
        timer = 0f;

        rigidbody.linearVelocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;
        rigidbody.gravityScale = 5f;

        spriteRenderer.enabled = true;

        gameObject.SetActive(false);
        container.Reload(this);
    }
}
