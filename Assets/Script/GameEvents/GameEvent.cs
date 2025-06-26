// GameEvent.cs
using UnityEngine;

// 这个特性让我们可以直接在Unity编辑器里右键创建事件资产
[CreateAssetMenu(fileName = "NewGameEvent", menuName = "Game Event System/Game Event", order = 1)]
public class GameEvent : ScriptableObject
{
    [Header("Basic Information")]
    public string eventName;
    [TextArea(3, 5)]
    public string description;

    [Header("Trigger Conditions")]
    [Tooltip("Minimum number of in-game days before the event can occur")]
    public int minGameDays = 0;
    [Tooltip("Maximum number of in-game days the event can occur (0 means no limit)")]
    public int maxGameDays = 0;

    [Header("Trigger Weight")]
    [Tooltip("Base weight of the event — higher weight increases the chance of being selected")]
    public float baseWeight = 10f;

    [Header("Event Effects")]
    [Tooltip("Actions to perform when the event is triggered")]
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