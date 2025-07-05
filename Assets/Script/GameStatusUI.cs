// GameStatusUI.cs - ������ľ����ʾ
using UnityEngine;
using TMPro;
using System.Text;

public class GameStatusUI : MonoBehaviour
{
    [Header("UIԪ������")]
    public TextMeshProUGUI gameStatusText;

    [Header("��Ҫ���ӵ�����")]
    public PopulationTier farmerTier;
    public ItemData fishData;
    public ItemData woodData; // <<< ��������Ӷ�ľ��ItemData������

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

        // --- ��ղ���ʼ�����ַ��� ---
        _statusBuilder.Clear();

        // --- �����˿ں��Ҹ��Ȳ��� ---
        if (farmerTier != null)
        {
            int farmerCount = PopulationManager.Instance.GetPopulation(farmerTier);
            float averageHappiness = PopulationManager.Instance.GetAverageHappiness(farmerTier);
            _statusBuilder.AppendLine($"farmer: {farmerCount}");
            _statusBuilder.AppendLine($"happiness: {averageHappiness:F1}");
        }

        // --- �������ʲ��� ---
        if (fishData != null)
        {
            float fishStock = ResourceManager.Instance.GetWarehouseStock(fishData);
            _statusBuilder.AppendLine($"fish: {fishStock:F0}");
        }

        // <<< ����������ľ����Ϣ���� >>>
        if (woodData != null)
        {
            float woodStock = ResourceManager.Instance.GetWarehouseStock(woodData);
            _statusBuilder.AppendLine($"wood: {woodStock:F0}");
        }

        // --- �������õ��ı�һ���Ը���UI ---
        gameStatusText.text = _statusBuilder.ToString();
    }
}