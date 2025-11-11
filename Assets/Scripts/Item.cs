using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemInfo itemInfo;

    private SpriteRenderer spriteRenderer;
    private bool isMove;
    private float timer = 0f;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (isMove)
        {
            Vector2 dir = Player.Instance.transform.position - transform.position;

            if (dir.sqrMagnitude > 0.1f)
            {
                float distance = dir.magnitude;

                // 플레이어에게 가까워질수록 더 빠르게 움직인다.
                float speed = Mathf.Lerp(1f, 10f, 1 - (distance / 10f));
                transform.position += (Vector3)(dir.normalized * speed * Time.deltaTime);
            }
            else
            {
                Disappear();
            }
        }

        // 일정 시간이 지나면 사라진다.
        if (timer > 60f)
        {
            Disappear(true);
        }

        timer += Time.deltaTime;
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
        timer = 0f;
    }
    private void OnDetected()
    {
        if (isMove)
        {
            return;
        }

        Vector3 targetPos = transform.position + new Vector3(0f, 0.1f, 0f);
        transform.DOMove(targetPos, 0.2f).OnComplete(() => 
        {
            isMove = true;
        });
    }
    private void Disappear(bool isTimeout = false)
    {
        if (!isTimeout)
        {
            Player.Instance.IncreaseExp(itemInfo.value);
        }

        ItemContainer.Instance.Reload(this);
        gameObject.SetActive(false);
        isMove = false;
        timer = 0f;
    }

    [System.Serializable]
    public struct ItemInfo
    {
        public int value;
        public Sprite sprite;
    }
}
