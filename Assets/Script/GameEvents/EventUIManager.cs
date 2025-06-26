// EventUIManager.cs
using UnityEngine;
using TMPro; // <--- 1. 引入TextMeshPro的命名空间
using System.Collections;

public class EventUIManager : MonoBehaviour
{
    [Header("UI Element")]
    public TextMeshProUGUI timerText;
    public GameObject eventNotificationPanel;
    public TextMeshProUGUI eventDescriptionText;

    [Header("Notification Display Time")]
    public float notificationDisplayTime = 5f; // 事件通知显示5秒

    // 在对象启用时，开始“收听”广播
    private void OnEnable()
    {
        EventDirector.OnTimerUpdated += UpdateTimerUI;
        EventDirector.OnEventTriggered += ShowEventNotification;
    }

    // 在对象禁用时，停止“收听”以防止内存泄漏
    private void OnDisable()
    {
        EventDirector.OnTimerUpdated -= UpdateTimerUI;
        EventDirector.OnEventTriggered -= ShowEventNotification;
    }

    // 更新计时器UI的函数
    private void UpdateTimerUI(float timeRemaining)
    {
        if (timerText != null)
        {
            // 确保时间不为负数，并格式化字符串
            timeRemaining = Mathf.Max(0, timeRemaining);
            timerText.text = $"next event: {timeRemaining:F1}"; // F1表示保留一位小数
        }
    }

    // 显示事件通知的函数
    private void ShowEventNotification(GameEvent triggeredEvent)
    {
        if (eventNotificationPanel != null && eventDescriptionText != null)
        {
            // 使用事件的名称和描述来填充UI
            eventDescriptionText.text = $"<b>{triggeredEvent.eventName}</b>\n\n{triggeredEvent.description}";

            // 启动一个协程来显示面板，并在几秒后自动隐藏
            StartCoroutine(DisplayNotificationPanel());
        }
    }

    // 用于显示和隐藏通知面板的协程
    private IEnumerator DisplayNotificationPanel()
    {
        // 显示面板
        eventNotificationPanel.SetActive(true);

        // 等待指定的时间
        yield return new WaitForSeconds(notificationDisplayTime);

        // 隐藏面板
        eventNotificationPanel.SetActive(false);
    }
}