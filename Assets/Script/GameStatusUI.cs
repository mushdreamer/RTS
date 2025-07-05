// GameStatusUI.cs - �����˲��ֺ�ͼ������
using UnityEngine;
using TMPro;
using System.Text; // ����StringBuilder����Чƴ���ַ���

public class GameStatusUI : MonoBehaviour
{
    // <<< �޸ģ���������ֻ��Ҫһ���ı�������ʾ������Ϣ >>>
    [Header("UIԪ������")]
    public TextMeshProUGUI gameStatusText;

    [Header("��Ҫ���ӵ�����")]
    public PopulationTier farmerTier;
    public ItemData fishData;

    // ����ʹ��StringBuilder��������Update��Ƶ���������ַ������������ܸ���
    private StringBuilder _statusBuilder = new StringBuilder();

    void Start()
    {
        if (ResourceManager.Instance != null)
        {
            // ����Դ�仯ʱ������ֱ�ӵ���UpdateUI��ˢ����������
            ResourceManager.Instance.OnResourceChanged += UpdateUI;
        }

        UpdateUI(); // ��ʼ��UI��ʾ
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
        // �˿ں��Ҹ��ȿ���ÿ֡���ڱ䣬����������Update��ˢ��
        UpdateUI();
    }

    /// <summary>
    /// һ��ͳһ�ķ��������ڸ�������״̬UI
    /// </summary>
    void UpdateUI()
    {
        // --- ��ȫ��� ---
        if (gameStatusText == null || farmerTier == null || fishData == null || PopulationManager.Instance == null || ResourceManager.Instance == null)
        {
            return;
        }

        // --- ��ȡ�������� ---
        int farmerCount = PopulationManager.Instance.GetPopulation(farmerTier);
        float averageHappiness = PopulationManager.Instance.GetAverageHappiness(farmerTier);
        float fishStock = ResourceManager.Instance.GetWarehouseStock(fishData);

        // --- ʹ��StringBuilder�������յ��ı� ---
        _statusBuilder.Clear(); // �����һ�ε�����

        // <<< �޸ģ�ȥ����sprite��ǩ����ʹ�� \n ���л��� >>>
        _statusBuilder.AppendLine($"Farmer: {farmerCount}");
        _statusBuilder.AppendLine($"Happiness: {averageHappiness:F1}");
        _statusBuilder.AppendLine($"Fish: {fishStock:F0}");

        // --- �������õ��ı�һ���Ը���UI ---
        gameStatusText.text = _statusBuilder.ToString();
    }
}