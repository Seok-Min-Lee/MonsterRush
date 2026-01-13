using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardDataAsset", menuName = "Scriptable Objects/RewardDataAsset")]
public class RewardDataAsset : ScriptableObject
{
    public IReadOnlyList<RewardInfo> Rewards => rewards;
    [SerializeField] private List<RewardInfo> rewards;
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
