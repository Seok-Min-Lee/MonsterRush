using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public static ItemContainer Instance;
    
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
        if (existItemBox && Random.Range(0, 100) == 0)
        {
            BatchItemBox(position);
        }
        else 
        {
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
        if (actives.Count > 200)
        {
            Reload(actives[0]);
        }

        Item item = pool.Count > 0 ?
                    pool.Dequeue() :
                    GameObject.Instantiate<Item>(prefab, transform);

        item.Init(itemInfo, position);
        actives.Add(item);
    }
    public void BatchItemBox(Vector3 position)
    {
        if (itemBoxes.Count > 10)
        {
            Reload(itemBoxes[0]);
        }

        ItemBox itemBox = itemBoxPool.Count > 0 ?
                          itemBoxPool.Dequeue() :
                          GameObject.Instantiate<ItemBox>(itemBoxPrefab, transform);

        itemBox.Init(dataContainer.specialItems[Random.Range(0, dataContainer.specialItems.Count)], position);
        itemBoxes.Add(itemBox);
    }
    public void Reload(Item item)
    {
        pool.Enqueue(item); 
        actives.Remove(item);
    }
    public void Reload(ItemBox itemBox)
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