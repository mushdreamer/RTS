// DayNightCycle.cs (Corrected Logic)
using UnityEngine;

[System.Serializable]
public class SeasonalSettings
{
    [Tooltip("该季节的日出时间（24小时制，例如 6.5 代表早上6:30）")]
    [Range(0, 24)]
    public float sunriseHour;

    [Tooltip("该季节的日落时间（24小时制，例如 18.0 代表晚上6:00）")]
    [Range(0, 24)]
    public float sunsetHour;
}

/// <summary>
/// 控制场景的昼夜视觉循环，现在支持季节性变化。
/// </summary>
public class DayNightCycle : MonoBehaviour
{
    [Header("场景引用")]
    [Tooltip("请将场景中的主光源（平行光，代表太阳）拖到这里")]
    [SerializeField] private Light sun;

    [Header("季节性设置")]
    [Tooltip("请按顺序为春、夏、秋、冬设置日出日落时间")]
    [SerializeField] private SeasonalSettings[] seasons = new SeasonalSettings[4];

    [Header("颜色设置")]
    [Tooltip("太阳光在一天中不同时间的颜色变化")]
    [SerializeField] private Gradient sunColor;

    [Tooltip("环境光在一天中不同时间的颜色变化")]
    [SerializeField] private Gradient ambientColor;

    private void Update()
    {
        if (TimeManager.Instance == null || seasons.Length != 4) return;

        // 1. 获取当前季节和精确时间
        TimeManager.Season currentSeason = TimeManager.Instance.CurrentSeason;
        SeasonalSettings currentSeasonSettings = seasons[(int)currentSeason];

        float currentHour = TimeManager.Instance.TimeOfDayNormalized * 24f;

        // 2. 使用修正后的函数，重新计算时间的视觉百分比
        float timePercent = CalculateVisualTimePercent(currentHour, currentSeasonSettings);

        // 3. 像以前一样更新视觉效果
        UpdateLighting(timePercent);
        UpdateSunRotation(timePercent);
    }

    /// <summary>
    /// 【已修正】根据季节性的日出日落时间，计算用于视觉表现的时间百分比
    /// </summary>
    private float CalculateVisualTimePercent(float currentHour, SeasonalSettings settings)
    {
        float sunrise = settings.sunriseHour;
        float sunset = settings.sunsetHour;

        // --- 白天逻辑 (从日出到日落) ---
        // 这一部分的逻辑是正确的，保持不变。
        if (currentHour >= sunrise && currentHour <= sunset)
        {
            // InverseLerp 计算当前时间在白天中的进度 (0 to 1)
            float dayProgress = Mathf.InverseLerp(sunrise, sunset, currentHour);
            // Lerp 将白天的进度映射到渐变色带的 25% (日出) 到 75% (日落)
            return Mathf.Lerp(0.25f, 0.75f, dayProgress);
        }

        // --- 夜晚逻辑 (从日落到第二天日出) ---
        // 这是修正后的新逻辑，更稳定可靠。
        float nightDuration;
        float timeSinceSunset;

        if (currentHour > sunset) // 情况A: 当天晚上 (例如: 18:00 - 23:59)
        {
            nightDuration = (24f - sunset) + sunrise;
            timeSinceSunset = currentHour - sunset;
        }
        else // 情况B: 第二天凌晨 (例如: 00:00 - 06:00)
        {
            nightDuration = (24f - sunset) + sunrise;
            timeSinceSunset = (24f - sunset) + currentHour;
        }

        // 计算当前时间在整个夜晚中的进度 (0 to 1)
        float nightProgress = timeSinceSunset / nightDuration;

        // 将夜晚的进度映射到渐变色带的 75% (日落) -> 100%/0% (午夜) -> 25% (日出)
        // 我们通过映射到 0.75 -> 1.25，然后取小数部分来实现这个循环
        float visualPercent = 0.75f + (nightProgress * 0.5f);
        return visualPercent % 1f;
    }


    private void UpdateLighting(float timePercent)
    {
        sun.color = sunColor.Evaluate(timePercent);
        RenderSettings.ambientLight = ambientColor.Evaluate(timePercent);
    }

    private void UpdateSunRotation(float timePercent)
    {
        // 旋转角度从-90(日出)到+90(日落)，午夜时太阳在正下方
        float sunAngle = Mathf.Lerp(-90, 270, timePercent);
        sun.transform.rotation = Quaternion.Euler(new Vector3(sunAngle, 170f, 0));
    }
}