using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class EnergyGridManager : MonoBehaviour
{
    public static EnergyGridManager Instance { get; private set; }

    // --- ����� PowerManager �б����Ĺ��� ---
    [Header("UI Elements")]
    [SerializeField] private Image sliderFill; //
    [SerializeField] private Slider powerSlider; //
    [SerializeField] private TextMeshProUGUI powerText; //

    [Header("Audio Clips")]
    public AudioClip powerInsufficientClip; //
    private AudioSource powerAudioSource; //
    private bool wasInsufficientLastFrame = false;

    // --- �µ�ϵͳ���� ---
    private List<IEnergyProducer> producers = new List<IEnergyProducer>();
    private List<IEnergyConsumer> consumers = new List<IEnergyConsumer>();

    // ���������ԣ���������ϵͳ����UI��ֱ�Ӷ�ȡ
    public float TotalProduction { get; private set; }
    public float TotalConsumption { get; private set; }
    public float AvailablePower { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // ��ʼ�������Ч���
        powerAudioSource = gameObject.AddComponent<AudioSource>();
        powerAudioSource.volume = 0.1f;
    }

    // --- �µ��Զ�ע��/ע������ ---
    public void RegisterProducer(IEnergyProducer producer)
    {
        if (!producers.Contains(producer)) producers.Add(producer);
    }

    public void UnregisterProducer(IEnergyProducer producer)
    {
        if (producers.Contains(producer)) producers.Remove(producer);
    }

    public void RegisterConsumer(IEnergyConsumer consumer)
    {
        if (!consumers.Contains(consumer)) consumers.Add(consumer);
    }

    public void UnregisterConsumer(IEnergyConsumer consumer)
    {
        if (consumers.Contains(consumer)) consumers.Remove(consumer);
    }

    // �����ڹ̶���ʱ�������µ�����������ÿһ֡
    void FixedUpdate()
    {
        UpdateEnergyGrid();
    }

    private void UpdateEnergyGrid()
    {
        // 1. �����ܷ�����
        TotalProduction = producers.Sum(p => p.CurrentProduction);

        // 2. ������������
        TotalConsumption = consumers.Sum(c => c.RequestedPower);

        // 3. ������õ���
        AvailablePower = TotalProduction - TotalConsumption;

        // 4. �������
        if (AvailablePower >= 0)
        {
            // �������㣬�����豸������������
            foreach (var consumer in consumers)
            {
                consumer.SupplyPower(consumer.RequestedPower);
            }
        }
        else
        {
            // �������㣬�����豸���ϵ�
            // (������Լ�������ӵ����ȼ��߼�)
            foreach (var consumer in consumers)
            {
                consumer.SupplyPower(0);
            }
        }

        // 5. ����UI�Ͳ�����Ч (������֮ǰд�õ��߼�)
        UpdatePowerUI();
        CheckAndPlaySounds();
    }

    // --- ���·����󲿷��Ǵ���� PowerManager.cs �����벢�����޸� ---
    private void UpdatePowerUI()
    {
        // ʹ���µĸ��������������߼�����ԭ����һ��
        if (AvailablePower > 0)
        {
            sliderFill.gameObject.SetActive(true);
        }
        else
        {
            sliderFill.gameObject.SetActive(false);
        }

        if (powerSlider != null)
        {
            powerSlider.maxValue = TotalProduction;
            powerSlider.value = AvailablePower;
        }

        if (powerText != null)
        {
            // ʹ�� Mathf.Max(0, ...) ������ʾ���������õ���
            float displayedUsage = Mathf.Max(0, TotalProduction - AvailablePower);
            powerText.text = $"{displayedUsage:F0}/{TotalProduction:F0}";
        }
    }

    private void CheckAndPlaySounds()
    {
        bool isInsufficientNow = AvailablePower < 0;

        // �����һ֡��ʼ�������㣬������һ֡�ǳ���ģ��򲥷���Ч
        // �������Ա���ÿһ֡�����š��������㡱������
        if (isInsufficientNow && !wasInsufficientLastFrame)
        {
            if (powerAudioSource && powerInsufficientClip)
            {
                powerAudioSource.PlayOneShot(powerInsufficientClip); //
            }
        }

        wasInsufficientLastFrame = isInsufficientNow;
    }
}