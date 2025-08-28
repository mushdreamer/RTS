// EventDirector.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class EventDirector : MonoBehaviour
{
    public static EventDirector Instance;

    [Header("Event Library")]
    [Tooltip("List of all possible game events")]
    public List<GameEvent> allEvents;

    // --- 以下计时器相关变量已被移除 ---
    // public float minTimeBetweenEvents = 20f;
    // public float maxTimeBetweenEvents = 60f;
    // private float timer;

    [Header("Current Game State (Example)")]
    // --- currentGameDay 已被移除，将从TimeManager获取 ---
    public int playerPopulation = 3; // 玩家人口
    // ... 在这里添加更多你需要追踪的游戏状态

    // --- OnTimerUpdated不再需要，但OnEventTriggered仍然有用 ---
    public static event Action<GameEvent> OnEventTriggered;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 当脚本启用时，订阅事件
    private void OnEnable()
    {
        TimeManager.OnDayChanged += HandleNewDay;
    }

    // 当脚本禁用时，取消订阅，防止内存泄漏
    private void OnDisable()
    {
        TimeManager.OnDayChanged -= HandleNewDay;
    }

    private void Start()
    {
        Debug.Log("EventDirector 已激活，正在等待新的一天...");
    }

    // --- Update() 方法中的计时逻辑已不再需要 ---

    /// <summary>
    /// 这个方法会在TimeManager广播OnDayChanged事件时被自动调用
    /// </summary>
    private void HandleNewDay(int newDay)
    {
        Debug.Log($"新的一天到来了 (第 {newDay} 天). 正在尝试触发随机事件...");
        TryTriggerEvent();
    }

    /// <summary>
    /// 尝试触发一个事件。核心的加权随机逻辑保持不变。
    /// </summary>
    public void TryTriggerEvent()
    {
        List<GameEvent> validEvents = allEvents.Where(e => e.AreConditionsMet(this)).ToList();

        if (validEvents.Count == 0)
        {
            Debug.Log("今天没有满足条件的事件可以触发。");
            return;
        }

        float totalWeight = validEvents.Sum(e => e.baseWeight);
        float randomPoint = UnityEngine.Random.Range(0, totalWeight);
        GameEvent chosenEvent = null;

        foreach (var e in validEvents)
        {
            if (randomPoint < e.baseWeight)
            {
                chosenEvent = e;
                break;
            }
            randomPoint -= e.baseWeight;
        }

        if (chosenEvent != null)
        {
            OnEventTriggered?.Invoke(chosenEvent);
            chosenEvent.Execute();
            Debug.Log($"事件已触发: [{chosenEvent.eventName}]");
        }
    }

    // --- 新增一个公共方法，用于给GameEvent查询当前的总天数 ---
    /// <summary>
    /// 从TimeManager获取当前游戏的总共通行天数
    /// </summary>
    public int GetCurrentTotalDays()
    {
        if (TimeManager.Instance != null)
        {
            return TimeManager.Instance.GetTotalDaysPassed();
        }

        Debug.LogWarning("无法找到TimeManager实例！返回天数为0。");
        return 0;
    }
}