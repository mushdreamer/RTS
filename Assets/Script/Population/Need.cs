// In folder: Assets/Scripts/Need.cs
[System.Serializable] // 这个特性让它可以在 Unity 编辑器中显示和编辑
public class Need
{
    public ItemData item;
    public float consumptionPerMinute; // 每分钟消耗量
    public bool isLuxuryNeed = false; // 标记是基本需求还是奢侈品需求
}