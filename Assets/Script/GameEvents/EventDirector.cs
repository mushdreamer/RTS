// EventDirector.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        ResetTimer();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            TryTriggerEvent();
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        timer = Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents);
        Debug.Log($"下次事件将在大约 {Mathf.RoundToInt(timer)} 秒后尝试触发。");
    }

    public void TryTriggerEvent()
    {
        // 1. 筛选出所有当前条件满足的事件
        List<GameEvent> validEvents = allEvents.Where(e => e.AreConditionsMet(this)).ToList();

        if (validEvents.Count == 0)
        {
            Debug.Log("没有满足条件的事件可以触发。");
            return;
        }

        // 2. 根据权重计算总权重
        float totalWeight = validEvents.Sum(e => e.baseWeight);

        // 3. 进行加权随机选择
        float randomPoint = Random.Range(0, totalWeight);
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

        // 4. 执行选中的事件
        if (chosenEvent != null)
        {
            chosenEvent.Execute();
        }
    }
}