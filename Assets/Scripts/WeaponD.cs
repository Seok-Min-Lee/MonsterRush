using DG.Tweening;
using System.Collections;
using UnityEngine;

public class WeaponD : Weapon
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private WeaponDFlare flare;

    [Header("Debug")]
    [SerializeField] private int knockbackLevel = 0;
    [SerializeField] private bool isScaleUp = false;

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
            OnExplosion();
        }
    }
    public void OnShot(WeaponContainerD container, bool isScaleUp, int knockbackLevel, Vector3 position, Vector3 force, float torque)
    {
        gameObject.SetActive(true);

        this.container = container;
        this.isScaleUp = isScaleUp;
        this.knockbackLevel = knockbackLevel;
        transform.position = position;
        renderer.sprite = isScaleUp ? sprites[1] : sprites[0];
        flare.Init(isScaleUp, knockbackLevel);

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

            renderer.enabled = false;

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

        renderer.enabled = true;

        gameObject.SetActive(false);
        container.Reload(this);
    }
}
