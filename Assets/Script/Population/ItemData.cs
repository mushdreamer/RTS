// In folder: Assets/Scripts/ItemData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item and Population Tier/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    // 你未来可以添加更多属性，比如物品分类、基础售价等
}
