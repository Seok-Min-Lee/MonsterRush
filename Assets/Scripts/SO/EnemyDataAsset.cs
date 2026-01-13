using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataAsset", menuName = "Scriptable Objects/EnemyDataAsset")]
public class EnemyDataAsset : ScriptableObject
{
    public IReadOnlyList<EnemyInfo> Enemies => enemies;
    [SerializeField] private List<EnemyInfo> enemies;
    public EnemyInfo SuperArmorEnemy => superArmorEnemy;
    [SerializeField] private EnemyInfo superArmorEnemy;
}
[Serializable]
public struct EnemyInfo
{
    public int hp;
    public int power;
    public float speed;
}
