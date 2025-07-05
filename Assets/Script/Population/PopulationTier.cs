// In folder: Assets/Scripts/PopulationTier.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Population Tier", menuName = "Item and Population Tier/Population Tier")]
public class PopulationTier : ScriptableObject
{
    [Header("阶层信息")]
    public string tierName;
    public int residentsPerHouse;

    [Header("需求列表")]
    public List<Need> needs;

    [Header("升级逻辑")]
    public PopulationTier nextTier; // <<< 新增：升级后的目标阶层
    public List<BuildRequirement> upgradeMaterials; // <<< 新增：升级需要的材料

    // 我们在这里添加一个简单的属性，来定义升级所需要的幸福度阈值
    public int HappinessToUpgrade => 11; // 意味着需要满幸福度才能升级
}

// BuildRequirement 类我们之前已经定义好了，现在可以复用它！
// 我们需要确保它在某个地方是可访问的，比如 ObjectsDatabseSO.cs 里
// 如果遇到 'BuildRequirement' not found 的错误，
// 我们可以把它的定义从 ObjectsDatabseSO.cs 移到一个单独的 BuildRequirement.cs 文件里。