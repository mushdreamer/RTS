// EventUIManager.cs (Final Version 2 - Corrected)
using UnityEngine;
using TMPro;
using System.Collections;

public class EventUIManager : MonoBehaviour
{
    [Header("时间显示UI")]
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;

    [Header("事件通知UI")]
    public GameObject eventNotificationPanel;
    public TextMeshProUGUI eventDescriptionText;

    [Header("通知显示时间")]
    public float notificationDisplayTime = 5f;

    private void OnEnable()
    {
        TimeManager.OnHourChanged += UpdateTimeUI;
        // 【修正】让所有与日期相关的事件都调用同一个处理方法
        TimeManager.OnDayChanged += HandleDateChanged;
        TimeManager.OnSeasonChanged += HandleDateChanged;
        TimeManager.OnYearChanged += HandleDateChanged;

        EventDirector.OnEventTriggered += ShowEventNotification;
    }

    private void OnDisable()
    {
        TimeManager.OnHourChanged -= UpdateTimeUI;
        // 【修正】同样，取消订阅同一个处理方法
        TimeManager.OnDayChanged -= HandleDateChanged;
        TimeManager.OnSeasonChanged -= HandleDateChanged;
        TimeManager.OnYearChanged -= HandleDateChanged;

        EventDirector.OnEventTriggered -= ShowEventNotification;
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            UpdateFullDateDisplay();
            UpdateTimeUI(TimeManager.Instance.Hour);
        }

        if (eventNotificationPanel != null)
        {
            eventNotificationPanel.SetActive(false);
        }
    }

    private void UpdateTimeUI(int hour)
    {
        if (timeText != null)
        {
            timeText.text = $"{hour:D2}:00";
        }
    }

    // --- 【修正】将所有重复的UpdateDateUI方法合并成下面这三个 ---

    // 当Day或Year变化时 (参数是int)，调用通用刷新方法
    private void HandleDateChanged(int value)
    {
        UpdateFullDateDisplay();
    }
    // 当Season变化时 (参数是Season)，调用通用刷新方法
    private void HandleDateChanged(TimeManager.Season season)
    {
        UpdateFullDateDisplay();
    }

    // 真正执行UI更新的通用方法
    private void UpdateFullDateDisplay()
    {
        if (dateText != null && TimeManager.Instance != null)
        {
            var tm = TimeManager.Instance;
            dateText.text = $"You survive {tm.Year} years and {TranslateSeason(tm.CurrentSeason)} {tm.Day} days";
        }
    }

    // --- 以下部分保持不变 ---
    private void ShowEventNotification(GameEvent triggeredEvent)
    {
        if (eventNotificationPanel != null && eventDescriptionText != null)
        {
            eventDescriptionText.text = $"<b>{triggeredEvent.eventName}</b>\n\n{triggeredEvent.description}";
            StartCoroutine(DisplayNotificationPanel());
        }
    }

    private IEnumerator DisplayNotificationPanel()
    {
        eventNotificationPanel.SetActive(true);
        yield return new WaitForSeconds(notificationDisplayTime);
        eventNotificationPanel.SetActive(false);
    }

    private string TranslateSeason(TimeManager.Season season)
    {
        switch (season)
        {
            case TimeManager.Season.Spring: return "Spring";
            case TimeManager.Season.Summer: return "Summer";
            case TimeManager.Season.Autumn: return "Fall";
            case TimeManager.Season.Winter: return "Winter";
            default: return "";
        }
    }
}