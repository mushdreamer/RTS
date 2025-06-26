// EventUIManager.cs
using UnityEngine;
using TMPro; // <--- 1. ����TextMeshPro�������ռ�
using System.Collections;

public class EventUIManager : MonoBehaviour
{
    [Header("UI Element")]
    public TextMeshProUGUI timerText;
    public GameObject eventNotificationPanel;
    public TextMeshProUGUI eventDescriptionText;

    [Header("Notification Display Time")]
    public float notificationDisplayTime = 5f; // �¼�֪ͨ��ʾ5��

    // �ڶ�������ʱ����ʼ���������㲥
    private void OnEnable()
    {
        EventDirector.OnTimerUpdated += UpdateTimerUI;
        EventDirector.OnEventTriggered += ShowEventNotification;
    }

    // �ڶ������ʱ��ֹͣ���������Է�ֹ�ڴ�й©
    private void OnDisable()
    {
        EventDirector.OnTimerUpdated -= UpdateTimerUI;
        EventDirector.OnEventTriggered -= ShowEventNotification;
    }

    // ���¼�ʱ��UI�ĺ���
    private void UpdateTimerUI(float timeRemaining)
    {
        if (timerText != null)
        {
            // ȷ��ʱ�䲻Ϊ����������ʽ���ַ���
            timeRemaining = Mathf.Max(0, timeRemaining);
            timerText.text = $"next event: {timeRemaining:F1}"; // F1��ʾ����һλС��
        }
    }

    // ��ʾ�¼�֪ͨ�ĺ���
    private void ShowEventNotification(GameEvent triggeredEvent)
    {
        if (eventNotificationPanel != null && eventDescriptionText != null)
        {
            // ʹ���¼������ƺ����������UI
            eventDescriptionText.text = $"<b>{triggeredEvent.eventName}</b>\n\n{triggeredEvent.description}";

            // ����һ��Э������ʾ��壬���ڼ�����Զ�����
            StartCoroutine(DisplayNotificationPanel());
        }
    }

    // ������ʾ������֪ͨ����Э��
    private IEnumerator DisplayNotificationPanel()
    {
        // ��ʾ���
        eventNotificationPanel.SetActive(true);

        // �ȴ�ָ����ʱ��
        yield return new WaitForSeconds(notificationDisplayTime);

        // �������
        eventNotificationPanel.SetActive(false);
    }
}