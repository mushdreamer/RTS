// GameStatusUI.cs - 增加了工人的显示
using UnityEngine;
using TMPro;
using System.Text;

public class GameStatusUI : MonoBehaviour
{
    [Header("UI元素引用")]
    public TextMeshProUGUI gameStatusText;

    [Header("需要监视的人口阶层")] // <<< Header 标题更新
    public PopulationTier farmerTier;
    public PopulationTier workerTier; // <<< 新增：添加对工人阶层(Tier_Worker)的引用

    [Header("需要监视的物资")] // <<< Header 标题更新
    public ItemData fishData;
    public ItemData woodData;

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

        _statusBuilder.Clear();

        // --- 构建人口部分 ---
        if (farmerTier != null)
        {
            int farmerCount = PopulationManager.Instance.GetPopulation(farmerTier);
            float farmerHappiness = PopulationManager.Instance.GetAverageHappiness(farmerTier);
            _statusBuilder.AppendLine($"Farmer: {farmerCount} | Happiness: {farmerHappiness:F1}");
        }

        // <<< 新增：构建工人的信息部分 >>>
        if (workerTier != null)
        {
            int workerCount = PopulationManager.Instance.GetPopulation(workerTier);
            // 我们暂时不计算工人的幸福度，只显示人口
            _statusBuilder.AppendLine($"Worker: {workerCount}");
        }

        // 添加一个空行作为分隔
        _statusBuilder.AppendLine();

        // --- 构建物资部分 ---
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

        gameStatusText.text = _statusBuilder.ToString();
    }
}