// PopulationUI.cs
using UnityEngine;
using TMPro;
using System.Text;

public class PopulationUI : MonoBehaviour
{
    [Header("UI元素引用")]
    public TextMeshProUGUI populationStatusText;

    [Header("需要监视的人口阶层")]
    public PopulationTier farmerTier;
    public PopulationTier workerTier;

    private StringBuilder _statusBuilder = new StringBuilder();

    // 在Update中持续更新UI以反映幸福度的实时变化
    void Update()
    {
        UpdatePopulationStatus();
    }

    void UpdatePopulationStatus()
    {
        if (populationStatusText == null || PopulationManager.Instance == null)
        {
            return;
        }

        _statusBuilder.Clear();

        // --- 构建农民信息 ---
        if (farmerTier != null)
        {
            int farmerCount = PopulationManager.Instance.GetPopulation(farmerTier);
            float farmerHappiness = PopulationManager.Instance.GetAverageHappiness(farmerTier);
            _statusBuilder.AppendLine($"Farmer: {farmerCount} | Happiness: {farmerHappiness:F1}");
        }

        // --- 构建工人信息 ---
        if (workerTier != null)
        {
            int workerCount = PopulationManager.Instance.GetPopulation(workerTier);
            float workerHappiness = PopulationManager.Instance.GetAverageHappiness(workerTier); // 假设工人也有幸福度
            _statusBuilder.AppendLine($"Worker: {workerCount} | Happiness: {workerHappiness:F1}");
        }

        populationStatusText.text = _statusBuilder.ToString();
    }
}