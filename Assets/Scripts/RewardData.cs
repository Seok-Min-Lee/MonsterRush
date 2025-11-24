using System;
using UnityEngine;

[Serializable]
public struct RewardInfo
{
    public enum Type { Weapon, Ability, Player, Buff, }
    public int groupId;
    public int index;
    public Sprite icon;
    public string head;
    public string desc;
    public Type type;

    public int UniqueKey => groupId + index;
}
