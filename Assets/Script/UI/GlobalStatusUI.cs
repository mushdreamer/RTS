// GlobalStatusUI.cs - 英文显示版本
using UnityEngine;
using TMPro;
using System.Text;

public class GlobalStatusUI : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    [Header("需要监视的人口阶层")]
    public PopulationTier farmerTier;
    public PopulationTier workerTier;

    private StringBuilder sb = new StringBuilder();

    void Update()
    {
        if (statusText == null || PopulationManager.Instance == null || WorkforceManager.Instance == null) return;

        sb.Clear();

        // --- 全局人口统计 (英文) ---
        int totalPop = PopulationManager.Instance.GetGrandTotalPopulation();
        int occupiedPop = WorkforceManager.Instance.GetTotalOccupiedWorkforce();
        int idlePop = totalPop - occupiedPop;

        // ▼▼▼【修改点】▼▼▼
        sb.AppendLine($"Total Population: {totalPop}");
        sb.AppendLine($"Working Population: {occupiedPop}");
        sb.AppendLine($"Idle Population: {idlePop}");
        sb.AppendLine("---");

        // --- 分阶层详细信息 (英文) ---
        if (farmerTier != null)
        {
            int tierTotal = PopulationManager.Instance.GetPopulation(farmerTier);
            int tierOccupied = WorkforceManager.Instance.GetOccupiedWorkforce(farmerTier);
            float tierHappiness = PopulationManager.Instance.GetAverageHappiness(farmerTier);

            // ▼▼▼【修改点】▼▼▼
            sb.AppendLine($"Farmers: {tierTotal} (Available: {tierTotal - tierOccupied}) | Happiness: {tierHappiness:F1}");
        }
        if (workerTier != null)
        {
            int tierTotal = PopulationManager.Instance.GetPopulation(workerTier);
            int tierOccupied = WorkforceManager.Instance.GetOccupiedWorkforce(workerTier);
            float tierHappiness = PopulationManager.Instance.GetAverageHappiness(workerTier);

            // ▼▼▼【修改点】▼▼▼
            sb.AppendLine($"Workers: {tierTotal} (Available: {tierTotal - tierOccupied}) | Happiness: {tierHappiness:F1}");
        }

        statusText.text = sb.ToString();
    }
}