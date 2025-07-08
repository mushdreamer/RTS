using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class EnergyGridManager : MonoBehaviour
{
    public static EnergyGridManager Instance { get; private set; }

    // --- 从你的 PowerManager 中保留的功能 ---
    [Header("UI Elements")]
    [SerializeField] private Image sliderFill; //
    [SerializeField] private Slider powerSlider; //
    [SerializeField] private TextMeshProUGUI powerText; //

    [Header("Audio Clips")]
    public AudioClip powerInsufficientClip; //
    private AudioSource powerAudioSource; //
    private bool wasInsufficientLastFrame = false;

    // --- 新的系统核心 ---
    private List<IEnergyProducer> producers = new List<IEnergyProducer>();
    private List<IEnergyConsumer> consumers = new List<IEnergyConsumer>();

    // 公开的属性，方便其他系统（或UI）直接读取
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

        // 初始化你的音效组件
        powerAudioSource = gameObject.AddComponent<AudioSource>();
        powerAudioSource.volume = 0.1f;
    }

    // --- 新的自动注册/注销方法 ---
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

    // 我们在固定的时间间隔更新电网，而不是每一帧
    void FixedUpdate()
    {
        UpdateEnergyGrid();
    }

    private void UpdateEnergyGrid()
    {
        // 1. 计算总发电量
        TotalProduction = producers.Sum(p => p.CurrentProduction);

        // 2. 计算总需求量
        TotalConsumption = consumers.Sum(c => c.RequestedPower);

        // 3. 计算可用电量
        AvailablePower = TotalProduction - TotalConsumption;

        // 4. 分配电力
        if (AvailablePower >= 0)
        {
            // 电力充足，所有设备都获得所需电力
            foreach (var consumer in consumers)
            {
                consumer.SupplyPower(consumer.RequestedPower);
            }
        }
        else
        {
            // 电力不足，所有设备都断电
            // (这里可以加入更复杂的优先级逻辑)
            foreach (var consumer in consumers)
            {
                consumer.SupplyPower(0);
            }
        }

        // 5. 更新UI和播放音效 (这是你之前写好的逻辑)
        UpdatePowerUI();
        CheckAndPlaySounds();
    }

    // --- 以下方法大部分是从你的 PowerManager.cs 中移入并稍作修改 ---
    private void UpdatePowerUI()
    {
        // 使用新的浮点数变量，但逻辑与你原来的一样
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
            // 使用 Mathf.Max(0, ...) 避免显示负数的已用电量
            float displayedUsage = Mathf.Max(0, TotalProduction - AvailablePower);
            powerText.text = $"{displayedUsage:F0}/{TotalProduction:F0}";
        }
    }

    private void CheckAndPlaySounds()
    {
        bool isInsufficientNow = AvailablePower < 0;

        // 如果这一帧开始电力不足，并且上一帧是充足的，则播放音效
        // 这样可以避免每一帧都播放“电力不足”的声音
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