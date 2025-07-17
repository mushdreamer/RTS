// HouseNeedState.cs
[System.Serializable]
public class HouseNeedState
{
    public Need associatedNeed; // 引用 ScriptableObject 的需求定义
    public bool isMet;          // 当前这个需求是否被满足

    public HouseNeedState(Need need)
    {
        associatedNeed = need;
        isMet = false; // 默认为不满足
    }
}