using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public static ItemContainer Instance;
    [SerializeField] Item prefab;
    private Queue<Item> pool = new Queue<Item>();

    private void Start()
    {
        Instance = this;
    }
    public void Batch(Vector3 position)
    {
        Item item = pool.Count > 0 ? 
                    pool.Dequeue() : 
                    GameObject.Instantiate<Item>(prefab, transform);

        item.Init(position);
    }
    public void Reload(Item item)
    {
        pool.Enqueue(item);
    }
}
