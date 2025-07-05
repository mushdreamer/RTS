// GameStatusUI.cs - 增加了木材显示
using UnityEngine;
using TMPro;
using System.Text;

public class GameStatusUI : MonoBehaviour
{
    [Header("UI元素引用")]
    public TextMeshProUGUI gameStatusText;

    [Header("需要监视的数据")]
    public PopulationTier farmerTier;
    public ItemData fishData;
    public ItemData woodData; // <<< 新增：添加对木材ItemData的引用

    private StringBuilder _statusBuilder = new StringBuilder();

    void Start()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += UpdateUI;
        }
        UpdateUI();
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
        UpdateUI();
    }

    void UpdateUI()
    {
        if (gameStatusText == null || PopulationManager.Instance == null || ResourceManager.Instance == null)
        {
            return;
        }

        // --- 清空并开始构建字符串 ---
        _statusBuilder.Clear();

        // --- 构建人口和幸福度部分 ---
        if (farmerTier != null)
        {
            int farmerCount = PopulationManager.Instance.GetPopulation(farmerTier);
            float averageHappiness = PopulationManager.Instance.GetAverageHappiness(farmerTier);
            _statusBuilder.AppendLine($"farmer: {farmerCount}");
            _statusBuilder.AppendLine($"happiness: {averageHappiness:F1}");
        }

        // --- 构建物资部分 ---
        if (fishData != null)
        {
            float fishStock = ResourceManager.Instance.GetWarehouseStock(fishData);
            _statusBuilder.AppendLine($"fish: {fishStock:F0}");
        }

        // <<< 新增：构建木材信息部分 >>>
        if (woodData != null)
        {
            float woodStock = ResourceManager.Instance.GetWarehouseStock(woodData);
            _statusBuilder.AppendLine($"wood: {woodStock:F0}");
        }

        // --- 将构建好的文本一次性赋给UI ---
        gameStatusText.text = _statusBuilder.ToString();
    }
}