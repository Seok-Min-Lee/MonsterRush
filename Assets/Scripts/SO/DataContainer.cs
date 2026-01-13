using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataContainer", menuName = "Scriptable Objects/DataContainer")]
public class DataContainer : ScriptableObject
{
    public List<RewardInfo> rewards;

    public ItemInfo healItem;
    public List<ItemInfo> expItems;
    public List<ItemInfo> specialItems;

    public List<EnemyInfo> enemies;
    public EnemyInfo superArmorEnemy;
}
[Serializable]
public struct RewardInfo
{
    public enum Type 
    {
        Weapon, Ability, Player,
    }

    public int groupId;
    public int index;
    public Sprite icon;
    public string head;
    public string desc;
    public Type type;

    public int UniqueKey => groupId + index;
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
[Serializable]
public struct EnemyInfo 
{
    public int hp;
    public int power;
    public float speed;
}
