// DebugLogAction.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewDebugLogAction", menuName = "Game Event System/Actions/Debug Log Action", order = 1)]
public class DebugLogAction : GameEventAction
{
    public string message;
    public override void Execute()
    {
        Debug.Log($"[Event Action]: {message}");
    }
}