// CityRulesManager.cs
using UnityEngine;

/// <summary>
/// 全局城市规则管理器。用于控制游戏核心机制的开关。
/// </summary>
public class CityRulesManager : MonoBehaviour
{
    /// <summary>
    /// 全局唯一的实例
    /// </summary>
    public static CityRulesManager Instance { get; private set; }

    [Header("游戏规则设置")]
    [Tooltip("如果勾选，城市中的建筑需要通过道路连接才能运作。如果取消勾选，所有建筑都将无视道路，视为自动连接。")]
    public bool requireRoadsForConnection = true;

    void Awake()
    {
        // 实现单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}