
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIContainer : MonoBehaviour
{
    public static UIContainer Instance { get; private set; }

    [SerializeField] private HitHealText prefab;

    private Queue<HitHealText> pool = new Queue<HitHealText>();
    private void Awake()
    {
        Instance = this;
    }
    public void Push(HitHealText child)
    {
        child.transform.parent = transform;
        pool.Enqueue(child);
    }
    public HitHealText Pop()
    {
        return pool.Count > 0 ? pool.Dequeue() : GameObject.Instantiate<HitHealText>(prefab);
    }
}
