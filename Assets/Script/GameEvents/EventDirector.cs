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

    [Header("Trigger Timing")]
    [Tooltip("Minimum interval (in seconds) before trying to trigger the next event")]
    public float minTimeBetweenEvents = 20f;
    [Tooltip("Maximum interval (in seconds) before trying to trigger the next event")]
    public float maxTimeBetweenEvents = 60f;

    [Header("Current Game State (Example)")]
    public int currentGameDay = 1; // 游戏进行的天数
    public int playerPopulation = 3; // 玩家人口
    // ... 在这里添加更多你需要追踪的游戏状态

    private float timer;

    // --- 2. 添加这两个公共静态事件 ---
    /// <summary>
    /// 当计时器更新时触发，广播剩余时间
    /// </summary>
    public static event Action<float> OnTimerUpdated;

    /// <summary>
    /// 当一个事件被选中并触发时，广播这个事件的数据
    /// </summary>
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

    private void Start()
    {
        // 在这里加入这行调试代码！
        /*Debug.Log($"当前计时器: {timer}");

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            TryTriggerEvent();
            ResetTimer();
        }*/
        ResetTimer();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        // --- 3. 在Update中广播计时器更新事件 ---
        OnTimerUpdated?.Invoke(timer);
        // ------------------------------------

        if (timer <= 0)
        {
            TryTriggerEvent();
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        timer = UnityEngine.Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents);
        Debug.Log($"Next event will be triggered after {Mathf.RoundToInt(timer)} seconds");
    }

    public void TryTriggerEvent()
    {
        // ... (筛选和加权随机选择的代码保持不变) ...
        List<GameEvent> validEvents = allEvents.Where(e => e.AreConditionsMet(this)).ToList();

        if (validEvents.Count == 0)
        {
            Debug.Log("No Event Condition!");
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
            // --- 4. 在执行事件前，广播事件触发信息 ---
            OnEventTriggered?.Invoke(chosenEvent);
            // ----------------------------------------

            // 然后再执行事件本身
            chosenEvent.Execute();
        }
    }
}