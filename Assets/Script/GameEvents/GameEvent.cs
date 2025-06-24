// GameEvent.cs
using UnityEngine;

// 这个特性让我们可以直接在Unity编辑器里右键创建事件资产
[CreateAssetMenu(fileName = "NewGameEvent", menuName = "Game Event System/Game Event", order = 1)]
public class GameEvent : ScriptableObject
{
    [Header("基本信息")]
    public string eventName;
    [TextArea(3, 5)]
    public string description;

    [Header("触发条件")]
    [Tooltip("事件可以发生的最小游戏天数")]
    public int minGameDays = 0;
    [Tooltip("事件可以发生的最大游戏天数（0表示无限制）")]
    public int maxGameDays = 0;

    [Header("触发权重")]
    [Tooltip("事件的基础权重，权重越高的事件越容易被选中")]
    public float baseWeight = 10f;

    [Header("事件效果")]
    [Tooltip("当事件触发时要执行的动作")]
    public GameEventAction[] actions;

    /// <summary>
    /// 检查当前游戏状态是否满足此事件的触发条件
    /// </summary>
    /// <param name="director">事件导演，用于获取当前游戏状态</param>
    /// <returns>如果满足条件则返回true</returns>
    public bool AreConditionsMet(EventDirector director)
    {
        // 检查游戏天数
        if (director.currentGameDay < minGameDays) return false;
        if (maxGameDays > 0 && director.currentGameDay > maxGameDays) return false;

        // 在这里可以添加更多的条件检查
        // 例如：检查玩家人口、财富、所在地图、已研究科技等
        // if (director.playerPopulation < 5) return false;

        return true;
    }

    /// <summary>
    /// 执行事件
    /// </summary>
    public void Execute()
    {
        Debug.Log($"事件触发: [{eventName}] - {description}");
        foreach (var action in actions)
        {
            action.Execute();
        }
    }
}