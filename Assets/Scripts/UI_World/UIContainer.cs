
using System.Collections.Generic;
using UnityEngine;

public class UIContainer : MonoBehaviour
{
    public static UIContainer Instance { get; private set; }

    [SerializeField] private CombatText prefab;

    private Queue<CombatText> pool = new Queue<CombatText>();
    private void Awake()
    {
        Instance = this;
    }
    public void Charge(CombatText child)
    {
        child.transform.parent = transform;
        pool.Enqueue(child);
    }
    public CombatText Discharge()
    {
        return pool.Count > 0 ? pool.Dequeue() : GameObject.Instantiate<CombatText>(prefab);
    }
}
