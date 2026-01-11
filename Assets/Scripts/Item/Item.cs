using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemInfo itemInfo;

    public bool isMove { get; private set; }

    private SpriteRenderer spriteRenderer;
    private Coroutine cor;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    IEnumerator MoveCor()
    {
        while (true)
        {
            Vector2 dir = Player.Instance.transform.position - transform.position;
            if (dir.sqrMagnitude < 0.1f)
            {
                break;
            }

            float distance = dir.magnitude;
            // 플레이어에게 가까워질수록 더 빠르게 움직인다.
            float speed = Mathf.Lerp(1f, 10f, 1 - (distance / 10f));
            transform.position += (Vector3)(dir.normalized * speed * Time.deltaTime);

            yield return null;
        }

        OnAbsorbed();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerArea"))
        {
            OnDetected();
        }
    }
    public void Init(ItemInfo itemInfo, Vector3 position)
    {
        this.itemInfo = itemInfo;
        spriteRenderer.sprite = itemInfo.sprite;
        transform.position = position;

        gameObject.SetActive(true);
        isMove = false;

        if (cor != null)
        {
            StopCoroutine(cor);
            cor = null;
        }
    }
    public void OnDetected()
    {
        if (isMove)
        {
            return;
        }

        Vector3 targetPos = transform.position + new Vector3(0f, 0.1f, 0f);
        transform.DOMove(targetPos, 0.2f).OnComplete(() => 
        {
            if (gameObject.activeSelf)
            {
                isMove = true;
                cor = StartCoroutine(MoveCor());
            }
        });
    }
    private void OnAbsorbed()
    {
        switch (itemInfo.type)
        {
            case ItemInfo.Type.Exp:
                Player.Instance.IncreaseExp(itemInfo.value);
                break;
            case ItemInfo.Type.Heal:
                Player.Instance.OnHeal(itemInfo.value);
                break;
            case ItemInfo.Type.HpUp:
                Player.Instance.IncreaseHp(5);
                break;
            case ItemInfo.Type.ExpBoost:
                Player.Instance.OnExpBoost();
                break;
            case ItemInfo.Type.Barrier:
                Player.Instance.OnBarrier();
                break;
            case ItemInfo.Type.PowerUp:
                Player.Instance.OnPowerUp();
                break;
            default:
                break;
        }

        AudioManager.Instance.PlaySFX(itemInfo.type == ItemInfo.Type.Exp ? SoundKey.PlayerGetItem : SoundKey.PlayerGetSpecialItem);
        ItemContainer.Instance.Charge(this);
        gameObject.SetActive(false);
        isMove = false;
    }
}
