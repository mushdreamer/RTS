using System;
using UnityEngine;

// 定义空气质量等级的枚举
public enum AirQualityLevel
{
    Excellent, // 优
    Good,      // 良
    Moderate,  // 中度污染
    Poor,      // 重度污染
    Hazardous  // 危险
}

public class AirQualityManager : MonoBehaviour
{
    // 单例模式，确保全局只有一个实例
    public static AirQualityManager Instance { get; private set; }

    [Header("状态")]
    [Tooltip("当前的二氧化碳浓度")]
    [SerializeField] private float currentCO2 = 0f;
    [Tooltip("当前的空气质量等级")]
    [SerializeField] private AirQualityLevel currentLevel = AirQualityLevel.Excellent;

    [Header("配置")]
    [Tooltip("二氧化碳的自然消散速率（单位/秒）")]
    [SerializeField] private float dissipationRate = 0.5f;

    // 定义不同空气质量等级的二氧化碳浓度阈值
    private readonly float[] thresholds = { 50f, 150f, 300f, 500f };

    // 事件：当空气质量等级发生变化时触发
    public event Action<AirQualityLevel> OnAirQualityChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        // 每秒钟自然消散一部分二氧化碳
        if (currentCO2 > 0)
        {
            currentCO2 -= dissipationRate * Time.deltaTime;
            currentCO2 = Mathf.Max(0, currentCO2); // 确保不会变为负数
            UpdateAirQualityLevel();
        }
    }

    /// <summary>
    /// 增加二氧化碳浓度（供煤电厂等污染源调用）
    /// </summary>
    /// <param name="amount">增加的量</param>
    public void AddCO2(float amount)
    {
        currentCO2 += amount;
        UpdateAirQualityLevel();

        // ▼▼▼ 添加下面这行代码 ▼▼▼
        Debug.Log($"接收到CO2，当前总量: {currentCO2}");
    }

    /// <summary>
    /// 获取当前的二氧化碳数值
    /// </summary>
    public float GetCurrentCO2()
    {
        return currentCO2;
    }

    /// <summary>
    /// 根据当前的二氧化碳浓度更新空气质量等级
    /// </summary>
    private void UpdateAirQualityLevel()
    {
        AirQualityLevel newLevel;

        if (currentCO2 < thresholds[0]) newLevel = AirQualityLevel.Excellent;
        else if (currentCO2 < thresholds[1]) newLevel = AirQualityLevel.Good;
        else if (currentCO2 < thresholds[2]) newLevel = AirQualityLevel.Moderate;
        else if (currentCO2 < thresholds[3]) newLevel = AirQualityLevel.Poor;
        else newLevel = AirQualityLevel.Hazardous;

        // 如果等级发生变化，则触发事件
        if (newLevel != currentLevel)
        {
            currentLevel = newLevel;
            OnAirQualityChanged?.Invoke(currentLevel);
            Debug.Log($"Air quality has changed to: {currentLevel}");
        }
    }
}