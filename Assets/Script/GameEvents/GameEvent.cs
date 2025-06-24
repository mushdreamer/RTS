// GameEvent.cs
using UnityEngine;

// ������������ǿ���ֱ����Unity�༭�����Ҽ������¼��ʲ�
[CreateAssetMenu(fileName = "NewGameEvent", menuName = "Game Event System/Game Event", order = 1)]
public class GameEvent : ScriptableObject
{
    [Header("������Ϣ")]
    public string eventName;
    [TextArea(3, 5)]
    public string description;

    [Header("��������")]
    [Tooltip("�¼����Է�������С��Ϸ����")]
    public int minGameDays = 0;
    [Tooltip("�¼����Է����������Ϸ������0��ʾ�����ƣ�")]
    public int maxGameDays = 0;

    [Header("����Ȩ��")]
    [Tooltip("�¼��Ļ���Ȩ�أ�Ȩ��Խ�ߵ��¼�Խ���ױ�ѡ��")]
    public float baseWeight = 10f;

    [Header("�¼�Ч��")]
    [Tooltip("���¼�����ʱҪִ�еĶ���")]
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