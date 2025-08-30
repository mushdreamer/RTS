using System;
using UnityEngine;

public class ResearchManager : MonoBehaviour
{
    // 单例模式
    public static ResearchManager Instance { get; private set; }

    // ▼▼▼【修正部分】▼▼▼
    [Header("状态 (在Inspector中查看)")]
    [SerializeField] private float currentFunding = 0f;
    // ▲▲▲【修正部分结束】▲▲▲

    public int CurrentLevel { get; private set; } = 1;

    [Header("配置")]
    [Tooltip("升级到2级所需的研究经费")]
    [SerializeField] private float fundingGoalForLevel2 = 5000f;

    // 事件
    public event Action<int> OnLevelUpgraded;
    public event Action<float, float> OnFundingChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public float GetCurrentFunding()
    {
        return currentFunding;
    }

    public float GetFundingGoal()
    {
        return fundingGoalForLevel2;
    }

    public void AddFunding(float amount)
    {
        if (CurrentLevel >= 2)
        {
            Debug.Log("Research Institute is already at max level.");
            return;
        }

        currentFunding += amount;
        OnFundingChanged?.Invoke(currentFunding, fundingGoalForLevel2);

        if (currentFunding >= fundingGoalForLevel2)
        {
            CurrentLevel = 2;
            OnLevelUpgraded?.Invoke(CurrentLevel);
            Debug.Log($"<color=green>Research Institute has been upgraded to Level {CurrentLevel}!</color>");
        }
    }
}