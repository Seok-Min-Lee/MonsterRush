using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public static ItemContainer Instance { get; private set; }

    [SerializeField] private Item prefab;
    [SerializeField] private ItemBox itemBoxPrefab;

    [SerializeField] private DataContainer dataContainer;

    private Queue<Item> pool = new Queue<Item>();
    private List<Item> actives = new List<Item>();

    private Queue<ItemBox> itemBoxPool = new Queue<ItemBox>();
    private List<ItemBox> itemBoxes = new List<ItemBox>();

    public bool existItemBox { get; private set; } = false;
    private void Awake()
    {
        Instance = this;
    }
    public void Batch(int index, Vector3 position)
    {
        // 낮은 확률로 아이템 박스 등장
        if (existItemBox && Random.Range(0, 100) == 0)
        {
            BatchItemBox(position);
        }
        else 
        {
            // 낮은 확률로 회복 아이템 등장
            if (Random.Range(0, 100) < 2)
            {
                Vector3 offset = (Vector3)(Random.insideUnitCircle.normalized * 0.1f);

                BatchItem(dataContainer.healItem, position + offset);
                BatchItem(dataContainer.expItems[index], position - offset);
            }
            else
            {
                BatchItem(dataContainer.expItems[index], position);
            }
        }
    }
    public void BatchItem(ItemInfo itemInfo, Vector3 position)
    {
        // 너무 많으면 가장 오래된 것 회수
        if (actives.Count > 200)
        {
            Charge(actives[0]);
        }

        // 배치
        Item item = pool.Count > 0 ?
                    pool.Dequeue() :
                    GameObject.Instantiate<Item>(prefab, transform);
        
        item.Init(itemInfo, position);
        actives.Add(item);
    }
    public void BatchItemBox(Vector3 position)
    {
        // 너무 많으면 가장 오래된 것 회수
        if (itemBoxes.Count > 10)
        {
            Charge(itemBoxes[0]);
        }

        // 배치
        ItemBox itemBox = itemBoxPool.Count > 0 ?
                          itemBoxPool.Dequeue() :
                          GameObject.Instantiate<ItemBox>(itemBoxPrefab, transform);

        itemBox.Init(dataContainer.specialItems[Random.Range(0, dataContainer.specialItems.Count)], position);
        itemBoxes.Add(itemBox);
    }
    public void Charge(Item item)
    {
        pool.Enqueue(item); 
        actives.Remove(item);
    }
    public void Charge(ItemBox itemBox)
    {
        itemBoxPool.Enqueue(itemBox);
        itemBoxes.Remove(itemBox);
    }
    public List<Item> GetUndetectedsAll()
    {
        List<Item> all = new List<Item>();

        for (int i = 0; i < actives.Count; i++)
        {
            if (!actives[i].isMove)
            {
                all.Add(actives[i]);
            }
        }

        return all;
    }
    public List<ItemBox> GetItemBoxAll()
    {
        List<ItemBox> boxes = new List<ItemBox>();

        for (int i = 0; i < itemBoxes.Count; i++)
        {
            if (itemBoxes[i].state != ItemBox.State.Open)
            {
                boxes.Add(itemBoxes[i]);
            }
        }
        return boxes;
    }
    public void OnExistItemBox(RewardInfo rewardInfo)
    {
        if (rewardInfo.groupId == 0 && rewardInfo.index == 96)
        {
            existItemBox = true;
        }
    }
}