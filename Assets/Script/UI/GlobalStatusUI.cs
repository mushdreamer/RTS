// GlobalStatusUI.cs - 英文显示版本 (已更新)
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
        // ▼▼▼【修改点】调用了新的方法 GetTotalAssignedWorkforce ▼▼▼
        int assignedPop = WorkforceManager.Instance.GetTotalAssignedWorkforce();
        int idlePop = totalPop - assignedPop;

        sb.AppendLine($"Total Population: {totalPop}");
        // ▼▼▼【修改点】文本描述更新为 "Assigned" ▼▼▼
        sb.AppendLine($"Assigned Population: {assignedPop}");
        sb.AppendLine($"Idle Population: {idlePop}");
        sb.AppendLine("---");

        // --- 分阶层详细信息 (英文) ---
        if (farmerTier != null)
        {
            int tierTotal = PopulationManager.Instance.GetPopulation(farmerTier);
            // ▼▼▼【修改点】调用了新的方法 GetAssignedWorkforce ▼▼▼
            int tierAssigned = WorkforceManager.Instance.GetAssignedWorkforce(farmerTier);
            float tierHappiness = PopulationManager.Instance.GetAverageHappiness(farmerTier);

            sb.AppendLine($"Farmers: {tierTotal} (Available: {tierTotal - tierAssigned}) | Happiness: {tierHappiness:F1}");
        }
        if (workerTier != null)
        {
            int tierTotal = PopulationManager.Instance.GetPopulation(workerTier);
            // ▼▼▼【修改点】调用了新的方法 GetAssignedWorkforce ▼▼▼
            int tierAssigned = WorkforceManager.Instance.GetAssignedWorkforce(workerTier);
            float tierHappiness = PopulationManager.Instance.GetAverageHappiness(workerTier);

            sb.AppendLine($"Workers: {tierTotal} (Available: {tierTotal - tierAssigned}) | Happiness: {tierHappiness:F1}");
        }

        statusText.text = sb.ToString();
    }
}