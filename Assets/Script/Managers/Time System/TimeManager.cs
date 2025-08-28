// TimeManager.cs
using UnityEngine;
using System;

/// <summary>
/// 游戏世界的主时钟，管理所有时间进阶和相关事件。
/// 建议将此脚本挂载到一个独立的、持久存在的GameObject上（例如 "Managers"）。
/// </summary>
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("时间换算比例")]
    [Tooltip("现实世界的多少秒，等于游戏世界的一小时")]
    public float secondsPerGameHour = 10f;

    [Header("游戏世界日历设定")]
    [Tooltip("一天有多少小时")]
    public const int HoursPerDay = 24;
    [Tooltip("一季有多少天")]
    public int daysPerSeason = 30;
    [Tooltip("一年有多少季")]
    public const int SeasonsPerYear = 4;

    [Header("当前时间（只读）")]
    [SerializeField] private int _year = 1;
    [SerializeField] private Season _season = Season.Spring;
    [SerializeField] private int _day = 1;
    [SerializeField] private int _hour = 0;

    // 内部计时器
    private float _currentTimeOfDayInSeconds;

    // --- 公共事件，供其他系统订阅 ---
    public static event Action<int> OnHourChanged;
    public static event Action<int> OnDayChanged;
    public static event Action<Season> OnSeasonChanged;
    public static event Action<int> OnYearChanged;

    // --- 公共属性，用于获取当前时间 ---
    public int Year => _year;
    public Season CurrentSeason => _season;
    public int Day => _day;
    public int Hour => _hour;

    // --- 在这里添加下面这行新代码 ---
    /// <summary>
    /// 获取当前时间在一整天中所占的百分比 (0.0 到 1.0)
    /// </summary>
    public float TimeOfDayNormalized => _currentTimeOfDayInSeconds / (secondsPerGameHour * HoursPerDay);

    // 季节的枚举
    public enum Season { Spring, Summer, Autumn, Winter }

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

    private void Update()
    {
        // 累加真实时间
        _currentTimeOfDayInSeconds += Time.deltaTime;

        // 计算当前是第几个小时
        int newHour = Mathf.FloorToInt(_currentTimeOfDayInSeconds / secondsPerGameHour);

        // 如果小时发生变化
        if (newHour != _hour)
        {
            _hour = newHour;
            OnHourChanged?.Invoke(_hour); // 广播小时变化事件

            // 小时到达24点，意味着新的一天开始
            if (_hour >= HoursPerDay)
            {
                _hour = 0;
                _currentTimeOfDayInSeconds = 0;
                AdvanceDay();
            }
        }
    }

    private void AdvanceDay()
    {
        _day++;
        OnDayChanged?.Invoke(_day); // 广播天数变化事件

        // 如果天数超过一季的总天数
        if (_day > daysPerSeason)
        {
            _day = 1;
            AdvanceSeason();
        }
    }

    private void AdvanceSeason()
    {
        _season++;
        if ((int)_season >= SeasonsPerYear)
        {
            _season = Season.Spring; // 季节循环
            AdvanceYear();
        }
        OnSeasonChanged?.Invoke(_season); // 广播季节变化事件
    }

    private void AdvanceYear()
    {
        _year++;
        OnYearChanged?.Invoke(_year); // 广播年份变化事件
    }

    /// <summary>
    /// 获取自游戏开始以来经过的总天数
    /// </summary>
    /// <returns>总天数</returns>
    public int GetTotalDaysPassed()
    {
        int daysFromYears = (_year - 1) * SeasonsPerYear * daysPerSeason;
        int daysFromSeasons = ((int)_season) * daysPerSeason;
        return daysFromYears + daysFromSeasons + (_day - 1);
    }
}