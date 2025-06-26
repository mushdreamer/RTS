// GameEvent.cs
using UnityEngine;

// ������������ǿ���ֱ����Unity�༭�����Ҽ������¼��ʲ�
[CreateAssetMenu(fileName = "NewGameEvent", menuName = "Game Event System/Game Event", order = 1)]
public class GameEvent : ScriptableObject
{
    [Header("Basic Information")]
    public string eventName;
    [TextArea(3, 5)]
    public string description;

    [Header("Trigger Conditions")]
    [Tooltip("Minimum number of in-game days before the event can occur")]
    public int minGameDays = 0;
    [Tooltip("Maximum number of in-game days the event can occur (0 means no limit)")]
    public int maxGameDays = 0;

    [Header("Trigger Weight")]
    [Tooltip("Base weight of the event �� higher weight increases the chance of being selected")]
    public float baseWeight = 10f;

    [Header("Event Effects")]
    [Tooltip("Actions to perform when the event is triggered")]
    public GameEventAction[] actions;

    /// <summary>
    /// ��鵱ǰ��Ϸ״̬�Ƿ�������¼��Ĵ�������
    /// </summary>
    /// <param name="director">�¼����ݣ����ڻ�ȡ��ǰ��Ϸ״̬</param>
    /// <returns>������������򷵻�true</returns>
    public bool AreConditionsMet(EventDirector director)
    {
        // �����Ϸ����
        if (director.currentGameDay < minGameDays) return false;
        if (maxGameDays > 0 && director.currentGameDay > maxGameDays) return false;

        // �����������Ӹ�����������
        // ���磺�������˿ڡ��Ƹ������ڵ�ͼ�����о��Ƽ���
        // if (director.playerPopulation < 5) return false;

        return true;
    }

    /// <summary>
    /// ִ���¼�
    /// </summary>
    public void Execute()
    {
        Debug.Log($"�¼�����: [{eventName}] - {description}");
        foreach (var action in actions)
        {
            action.Execute();
        }
    }
}