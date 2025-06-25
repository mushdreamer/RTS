// EventDirector.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class EventDirector : MonoBehaviour
{
    public static EventDirector Instance;

    [Header("�¼���")]
    [Tooltip("���п��ܷ�������Ϸ�¼��б�")]
    public List<GameEvent> allEvents;

    [Header("����ʱ��")]
    [Tooltip("�´γ��Դ����¼�����С������룩")]
    public float minTimeBetweenEvents = 20f;
    [Tooltip("�´γ��Դ����¼�����������룩")]
    public float maxTimeBetweenEvents = 60f;

    [Header("��ǰ��Ϸ״̬ (ʾ��)")]
    public int currentGameDay = 1; // ��Ϸ���е�����
    public int playerPopulation = 3; // ����˿�
    // ... ��������Ӹ�������Ҫ׷�ٵ���Ϸ״̬

    private float timer;

    // --- 2. ���������������̬�¼� ---
    /// <summary>
    /// ����ʱ������ʱ�������㲥ʣ��ʱ��
    /// </summary>
    public static event Action<float> OnTimerUpdated;

    /// <summary>
    /// ��һ���¼���ѡ�в�����ʱ���㲥����¼�������
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
        // ������������е��Դ��룡
        /*Debug.Log($"��ǰ��ʱ��: {timer}");

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

        // --- 3. ��Update�й㲥��ʱ�������¼� ---
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
        Debug.Log($"�´��¼����ڴ�Լ {Mathf.RoundToInt(timer)} ����Դ�����");
    }

    public void TryTriggerEvent()
    {
        // ... (ɸѡ�ͼ�Ȩ���ѡ��Ĵ��뱣�ֲ���) ...
        List<GameEvent> validEvents = allEvents.Where(e => e.AreConditionsMet(this)).ToList();

        if (validEvents.Count == 0)
        {
            Debug.Log("û�������������¼����Դ�����");
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
            // --- 4. ��ִ���¼�ǰ���㲥�¼�������Ϣ ---
            OnEventTriggered?.Invoke(chosenEvent);
            // ----------------------------------------

            // Ȼ����ִ���¼�����
            chosenEvent.Execute();
        }
    }
}