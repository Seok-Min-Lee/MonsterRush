using System;
using UnityEngine;

[Serializable]
public struct RewardInfo
{
    public int groupId;
    public int index;
    public Sprite icon;
    public string head;
    public string desc;
    public bool isSpecial;
}
