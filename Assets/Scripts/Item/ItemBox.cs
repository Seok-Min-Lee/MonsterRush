using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public enum State { Normal, Hit, Open }
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Sprite[] opens;
    [SerializeField] private GaugeBar hpGaugeBar;

    private BoxCollider2D collider;
    private ItemInfo itemInfo;

    public State state { get; private set; } = State.Normal;
    private int hp = 5;
    private int hpMax = 5;
    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }
    public void Init(ItemInfo itemInfo, Vector3 position)
    {
        this.itemInfo = itemInfo;

        renderer.sprite = opens[0];
        renderer.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f);

        transform.position = position;

        hp = hpMax;
        hpGaugeBar.Init(maxValue: hpMax, currentValue: hp, visible: false);

        collider.enabled = true;
        state = State.Normal;
        gameObject.SetActive(true);
    }
    public void onHit()
    {
        if (state == State.Normal)
        {
            if (--hp > 0)
            {
                hpGaugeBar.SetValue(hp);
                renderer.transform.DOShakeScale(0.15f, 0.1f, 3, 1, true).OnComplete(() => { state = State.Normal; });
            }
            else
            {
                onOpen();
            }
        }
    }
    public void onOpen()
    {
        StartCoroutine(Cor());

        IEnumerator Cor()
        {
            state = State.Open;
            collider.enabled = false;

            for (int i = 0; i < opens.Length; i++)
            {
                renderer.sprite = opens[i];
                yield return null;
            }

            ItemContainer.Instance.BatchItem(itemInfo, transform.position);
            ItemContainer.Instance.Charge(this);
            gameObject.SetActive(false);
        }
    }
}
