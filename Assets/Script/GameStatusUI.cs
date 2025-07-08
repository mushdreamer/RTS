// GameStatusUI.cs - �����˹��˵���ʾ
using UnityEngine;
using TMPro;
using System.Text;

public class GameStatusUI : MonoBehaviour
{
    [Header("UIԪ������")]
    public TextMeshProUGUI gameStatusText;

    [Header("��Ҫ���ӵ��˿ڽײ�")] // <<< Header �������
    public PopulationTier farmerTier;
    public PopulationTier workerTier; // <<< ��������ӶԹ��˽ײ�(Tier_Worker)������

    [Header("��Ҫ���ӵ�����")] // <<< Header �������
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

        // --- �����˿ڲ��� ---
        if (farmerTier != null)
        {
            int farmerCount = PopulationManager.Instance.GetPopulation(farmerTier);
            float farmerHappiness = PopulationManager.Instance.GetAverageHappiness(farmerTier);
            _statusBuilder.AppendLine($"Farmer: {farmerCount} | Happiness: {farmerHappiness:F1}");
        }

        // <<< �������������˵���Ϣ���� >>>
        if (workerTier != null)
        {
            int workerCount = PopulationManager.Instance.GetPopulation(workerTier);
            // ������ʱ�����㹤�˵��Ҹ��ȣ�ֻ��ʾ�˿�
            _statusBuilder.AppendLine($"Worker: {workerCount}");
        }

        // ���һ��������Ϊ�ָ�
        _statusBuilder.AppendLine();

        // --- �������ʲ��� ---
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