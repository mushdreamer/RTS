// GameEventAction.cs
using UnityEngine;

// ����һ�������࣬�����������¼������Ĺ淶
public abstract class GameEventAction : ScriptableObject
{
    /// <summary>
    /// ִ�д˶����ľ����߼�
    /// </summary>
    public abstract void Execute();
}
