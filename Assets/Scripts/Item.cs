using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemInfo itemInfo;

    private SpriteRenderer spriteRenderer;
    private bool isMove;
    private Coroutine coroutine;
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

                float speed = Mathf.Lerp(1f, 10f, 1 - (distance / 10f));
                transform.position += (Vector3)(dir.normalized * speed * Time.deltaTime);
            }
            else
            {
                Disappear();
            }
        }
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
    }
    private void OnDetected()
    {
        if (isMove)
        {
            return;
        }

        coroutine = StartCoroutine(Cor());

        IEnumerator Cor()
        {
            float t = 0f;

            Vector3 target = transform.position + new Vector3(0f, .1f, 0f);

            while (t < 1f)
            {
                t += Time.deltaTime * 5;
                transform.position = Vector3.Lerp(transform.position, target, t);

                yield return null;
            }

            isMove = true;
        }
    }
    private void Disappear()
    {
        StopCoroutine(coroutine);

        Player.Instance.GainExp(itemInfo.value);
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
