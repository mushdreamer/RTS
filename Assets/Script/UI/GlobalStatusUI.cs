// GlobalStatusUI.cs - 优化版
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

        // --- 1. 全局总览统计 ---
        int grandTotalPop = PopulationManager.Instance.GetGrandTotalPopulation();
        int grandTotalAssigned = WorkforceManager.Instance.GetTotalAssignedWorkforce();
        int grandTotalIdle = grandTotalPop - grandTotalAssigned;

        sb.AppendLine("--- Global Status ---");
        sb.AppendLine($"Total Population: {grandTotalPop}");
        sb.AppendLine($"Assigned Population: {grandTotalAssigned}");
        sb.AppendLine($"Idle Population: {grandTotalIdle}");
        sb.AppendLine("---");

        // --- 2. 分阶层详细信息 (农民) ---
        if (farmerTier != null)
        {
            sb.AppendLine($"--- {farmerTier.tierName} ---"); // 使用 tierName 动态显示阶级名

            int tierTotal = PopulationManager.Instance.GetPopulation(farmerTier);
            int tierAssigned = WorkforceManager.Instance.GetAssignedWorkforce(farmerTier);
            int tierIdle = tierTotal - tierAssigned; // 计算该阶层的闲置人口
            float tierHappiness = PopulationManager.Instance.GetAverageHappiness(farmerTier);

            sb.AppendLine($"Total: {tierTotal}");
            sb.AppendLine($"Assigned: {tierAssigned}");
            sb.AppendLine($"Idle: {tierIdle}"); // 明确显示闲置人口
            sb.AppendLine($"Happiness: {tierHappiness:F1}");
            sb.AppendLine("---");
        }

        // --- 3. 分阶层详细信息 (工人) ---
        if (workerTier != null)
        {
            sb.AppendLine($"--- {workerTier.tierName} ---"); // 使用 tierName 动态显示阶级名

            int tierTotal = PopulationManager.Instance.GetPopulation(workerTier);
            int tierAssigned = WorkforceManager.Instance.GetAssignedWorkforce(workerTier);
            int tierIdle = tierTotal - tierAssigned; // 计算该阶层的闲置人口
            float tierHappiness = PopulationManager.Instance.GetAverageHappiness(workerTier);

            sb.AppendLine($"Total: {tierTotal}");
            sb.AppendLine($"Assigned: {tierAssigned}");
            sb.AppendLine($"Idle: {tierIdle}"); // 明确显示闲置人口
            sb.AppendLine($"Happiness: {tierHappiness:F1}");
            sb.AppendLine("---");
        }

        statusText.text = sb.ToString();
    }
}