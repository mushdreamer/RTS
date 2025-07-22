// GlobalStatusUI.cs
using UnityEngine;
using TMPro;
using System.Text;

public class GlobalStatusUI : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    public PopulationTier farmerTier;
    public PopulationTier workerTier;

    private StringBuilder sb = new StringBuilder();

    void Update()
    {
        if (statusText == null || PopulationManager.Instance == null || WorkforceManager.Instance == null) return;

        sb.Clear();

        int totalPop = PopulationManager.Instance.GetGrandTotalPopulation();
        int occupiedPop = WorkforceManager.Instance.GetTotalOccupiedWorkforce();
        int idlePop = totalPop - occupiedPop;

        sb.AppendLine($"总人口: {totalPop}");
        sb.AppendLine($"工作人口: {occupiedPop}");
        sb.AppendLine($"闲置人口: {idlePop}");
        sb.AppendLine("---");

        if (farmerTier != null)
        {
            int tierTotal = PopulationManager.Instance.GetPopulation(farmerTier);
            int tierOccupied = WorkforceManager.Instance.GetOccupiedWorkforce(farmerTier);
            sb.AppendLine($"可用农民: {tierTotal - tierOccupied}");
        }
        if (workerTier != null)
        {
            int tierTotal = PopulationManager.Instance.GetPopulation(workerTier);
            int tierOccupied = WorkforceManager.Instance.GetOccupiedWorkforce(workerTier);
            sb.AppendLine($"可用工人: {tierTotal - tierOccupied}");
        }

        statusText.text = sb.ToString();
    }
}