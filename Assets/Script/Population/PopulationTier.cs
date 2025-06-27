// In folder: Assets/Scripts/PopulationTier.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Population Tier", menuName = "Item and Population Tier/Population Tier")]
public class PopulationTier : ScriptableObject
{
    [Header("阶层信息")]
    public string tierName; // 例如 "农民"
    public int residentsPerHouse; // 每个房屋容纳的居民数量

    [Header("需求列表")]
    public List<Need> needs; // 该阶层的所有需求

    [Header("升级逻辑")]
    public PopulationTier nextTier; // 升级后的目标阶层 (现在可以留空)
    public List<ItemData> upgradeMaterials; // 升级需要的材料 (现在可以留空)
}
