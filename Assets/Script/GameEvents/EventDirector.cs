// EventDirector.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        Debug.Log($"�´��¼����ڴ�Լ {Mathf.RoundToInt(timer)} ����Դ�����");
    }

    public void TryTriggerEvent()
    {
        // 1. ɸѡ�����е�ǰ����������¼�
        List<GameEvent> validEvents = allEvents.Where(e => e.AreConditionsMet(this)).ToList();

        if (validEvents.Count == 0)
        {
            Debug.Log("û�������������¼����Դ�����");
            return;
        }

        // 2. ����Ȩ�ؼ�����Ȩ��
        float totalWeight = validEvents.Sum(e => e.baseWeight);

        // 3. ���м�Ȩ���ѡ��
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

        // 4. ִ��ѡ�е��¼�
        if (chosenEvent != null)
        {
            chosenEvent.Execute();
        }
    }
}