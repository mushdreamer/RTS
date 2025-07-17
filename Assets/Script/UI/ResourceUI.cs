// ResourceUI.cs (修改后版本)
using UnityEngine;
using TMPro;
using System.Text;

public class ResourceUI : MonoBehaviour
{
    [Header("UI元素引用")]
    public TextMeshProUGUI resourceStatusText;

    [Header("需要监视的物资")]
    public ItemData fishData;
    public ItemData woodData;

    private StringBuilder _statusBuilder = new StringBuilder();

    // 修改点 1: 使用 OnEnable 替代 Start
    // 当这个UI组件被激活时（即父面板被激活时），订阅事件并更新
    void OnEnable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += UpdateResourceStatus;
        }
        // 立即执行一次以显示当前值
        UpdateResourceStatus();
    }

    // 修改点 2: 使用 OnDisable 替代 OnDestroy
    // 当这个UI组件被禁用时（即父面板被隐藏时），取消订阅
    void OnDisable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= UpdateResourceStatus;
        }
    }

    void UpdateResourceStatus()
    {
        if (resourceStatusText == null || ResourceManager.Instance == null)
        {
            return;
        }

        _statusBuilder.Clear();

        if (fishData != null)
        {
            float fishStock = ResourceManager.Instance.GetWarehouseStock(fishData);
            _statusBuilder.AppendLine($"Fish: {fishStock:F0}");
        }

        if (woodData != null)
        {
            float woodStock = ResourceManager.Instance.GetWarehouseStock(woodData);
            _statusBuilder.AppendLine($"Wood: {woodStock:F0}");
        }

        resourceStatusText.text = _statusBuilder.ToString();
    }
}