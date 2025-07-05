// GameStatusUI.cs - 修正了布局和图标问题
using UnityEngine;
using TMPro;
using System.Text; // 引入StringBuilder，高效拼接字符串

public class GameStatusUI : MonoBehaviour
{
    // <<< 修改：我们现在只需要一个文本框来显示所有信息 >>>
    [Header("UI元素引用")]
    public TextMeshProUGUI gameStatusText;

    [Header("需要监视的数据")]
    public PopulationTier farmerTier;
    public ItemData fishData;

    // 我们使用StringBuilder来避免在Update中频繁创建新字符串，这样性能更好
    private StringBuilder _statusBuilder = new StringBuilder();

    void Start()
    {
        if (ResourceManager.Instance != null)
        {
            // 当资源变化时，我们直接调用UpdateUI来刷新所有内容
            ResourceManager.Instance.OnResourceChanged += UpdateUI;
        }

        UpdateUI(); // 初始化UI显示
    }

    void OnDestroy()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= UpdateUI;
        }
    }

    void Update()
    {
        // 人口和幸福度可能每帧都在变，所以我们在Update里刷新
        UpdateUI();
    }

    /// <summary>
    /// 一个统一的方法，用于更新所有状态UI
    /// </summary>
    void UpdateUI()
    {
        // --- 安全检查 ---
        if (gameStatusText == null || farmerTier == null || fishData == null || PopulationManager.Instance == null || ResourceManager.Instance == null)
        {
            return;
        }

        // --- 获取最新数据 ---
        int farmerCount = PopulationManager.Instance.GetPopulation(farmerTier);
        float averageHappiness = PopulationManager.Instance.GetAverageHappiness(farmerTier);
        float fishStock = ResourceManager.Instance.GetWarehouseStock(fishData);

        // --- 使用StringBuilder构建最终的文本 ---
        _statusBuilder.Clear(); // 清空上一次的内容

        // <<< 修改：去掉了sprite标签，并使用 \n 进行换行 >>>
        _statusBuilder.AppendLine($"Farmer: {farmerCount}");
        _statusBuilder.AppendLine($"Happiness: {averageHappiness:F1}");
        _statusBuilder.AppendLine($"Fish: {fishStock:F0}");

        // --- 将构建好的文本一次性赋给UI ---
        gameStatusText.text = _statusBuilder.ToString();
    }
}