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
        Player.Instance.IncreaseExp(itemInfo.value);
        ItemContainer.Instance.Reload(this);
        gameObject.SetActive(false);
        isMove = false;
    }

    [System.Serializable]
    public struct ItemInfo
    {
        public int value;
        public Sprite sprite;
    }
}
