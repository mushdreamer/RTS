// EventDirector.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class EventDirector : MonoBehaviour
{
    public static EventDirector Instance;

    [Header("事件库")]
    [Tooltip("所有可能发生的游戏事件列表")]
    public List<GameEvent> allEvents;

    [Header("触发时机")]
    [Tooltip("下次尝试触发事件的最小间隔（秒）")]
    public float minTimeBetweenEvents = 20f;
    [Tooltip("下次尝试触发事件的最大间隔（秒）")]
    public float maxTimeBetweenEvents = 60f;

    [Header("当前游戏状态 (示例)")]
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
        Debug.Log($"下次事件将在大约 {Mathf.RoundToInt(timer)} 秒后尝试触发。");
    }

    public void TryTriggerEvent()
    {
        // ... (筛选和加权随机选择的代码保持不变) ...
        List<GameEvent> validEvents = allEvents.Where(e => e.AreConditionsMet(this)).ToList();

        if (validEvents.Count == 0)
        {
            Debug.Log("没有满足条件的事件可以触发。");
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