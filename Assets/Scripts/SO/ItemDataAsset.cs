using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataAsset", menuName = "Scriptable Objects/ItemDataAsset")]
public class ItemDataAsset : ScriptableObject
{
    public ItemInfo HealItem => healItem;
    [SerializeField] private ItemInfo healItem;
    public IReadOnlyList<ItemInfo> ExpItems => expItems;
    [SerializeField] private List<ItemInfo> expItems;
    public IReadOnlyList<ItemInfo> SpecialItems => specialItems;
    [SerializeField] private List<ItemInfo> specialItems;
    public ItemInfo randomSpecialItem => specialItems[UnityEngine.Random.Range(0, specialItems.Count)];
}
[Serializable]
public struct ItemInfo
{
    public enum Type
    {
        Exp, Heal, HpUp, ExpBoost, Barrier, PowerUp
    }

    public Type type;
    public int value;
    public Sprite sprite;
}
