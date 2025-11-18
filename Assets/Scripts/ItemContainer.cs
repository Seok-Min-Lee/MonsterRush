using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public static ItemContainer Instance;
    [SerializeField] private Item prefab;
    [SerializeField] private Item.ItemInfo[] itemInfoes;
    private Queue<Item> pool = new Queue<Item>();
    private List<Item> actives = new List<Item>();
    private void Start()
    {
        Instance = this;
    }
    public void Batch(int index, Vector3 position)
    {
        if (actives.Count > 200)
        {
            Reload(actives[0]);
        }

        Item item = pool.Count > 0 ?
                    pool.Dequeue() :
                    GameObject.Instantiate<Item>(prefab, transform);

        item.Init(itemInfoes[index], position);
        actives.Add(item);
    }
    public void Reload(Item item)
    {
        pool.Enqueue(item); 
        actives.Remove(item);
    }
}