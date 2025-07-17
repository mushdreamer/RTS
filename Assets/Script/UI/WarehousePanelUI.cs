// WarehousePanelUI.cs
using UnityEngine;

public class WarehousePanelUI : MonoBehaviour
{
    [Header("UI引用")]
    [Tooltip("需要控制显示和隐藏的资源UI面板")]
    public GameObject resourcePanel; // 指向包含ResourceUI的那个面板

    void Start()
    {
        // 确保我们能找到单位选择管理器
        if (UnitSelectionManager.Instance != null)
        {
            // 订阅“选择已改变”事件
            UnitSelectionManager.Instance.OnSelectionChanged += HandleSelectionChanged;
        }
        else
        {
            Debug.LogError("WarehousePanelUI 无法找到 UnitSelectionManager 的实例！");
        }

        // 默认隐藏面板
        if (resourcePanel != null)
        {
            resourcePanel.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // 组件销毁时取消订阅
        if (UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.OnSelectionChanged -= HandleSelectionChanged;
        }
    }

    /// <summary>
    /// 当玩家的选择发生变化时，此方法被调用
    /// </summary>
    private void HandleSelectionChanged(GameObject newSelection)
    {
        // 检查UI面板是否已设置
        if (resourcePanel == null) return;

        // 检查新选择的物体上是否有 "Warehouse" 组件
        if (newSelection != null && newSelection.TryGetComponent<Warehouse>(out _))
        {
            // 如果是仓库，则显示资源面板
            resourcePanel.SetActive(true);
        }
        else
        {
            // 如果不是仓库（或取消选择），则隐藏资源面板
            resourcePanel.SetActive(false);
        }
    }
}