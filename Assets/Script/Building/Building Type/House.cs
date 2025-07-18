// House.cs - 完整修正版
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class House : MonoBehaviour
{
    public PopulationTier currentTier;

    [Header("实时状态")]
    public List<HouseNeedState> trackedNeeds;
    public int currentResidents;
    public int maxResidents;
    public int currentHappiness;
    // ▼▼▼【错误来源1】您的代码里缺少这个变量 ▼▼▼
    public bool isConnectedToWarehouse = false;

    [Header("成长参数")]
    public int baseResidents = 5;
    public int residentsPerNeedMet = 5;

    private bool _isActivated = false;
    // ▼▼▼【错误来源2】您的代码里缺少这个变量 ▼▼▼
    private Constructable constructable;

    private Vector3Int _myGridPosition; // 【新增】用来存储自己准确的网格坐标

    public void ActivateHouse(Vector3Int gridPosition)
    {
        if (_isActivated) return;

        _myGridPosition = gridPosition; // 【新增】保存这个坐标

        // ▼▼▼【错误来源3】缺少对constructable的赋值 ▼▼▼
        constructable = GetComponent<Constructable>();
        if (constructable == null)
        {
            Debug.LogError("House脚本无法找到Constructable组件！");
            return;
        }

        PopulationManager.Instance.RegisterHouse(this);
        InitializeNeeds();
        RecalculateState();
        _isActivated = true;
    }

    void OnDestroy()
    {
        if (_isActivated && PopulationManager.Instance != null)
        {
            PopulationManager.Instance.UnregisterHouse(this);
        }
    }

    private void InitializeNeeds()
    {
        trackedNeeds = new List<HouseNeedState>();
        if (currentTier != null && currentTier.needs != null)
        {
            foreach (var need in currentTier.needs)
            {
                trackedNeeds.Add(new HouseNeedState(need));
            }
        }
    }

    public void RecalculateState()
    {
        if (currentTier == null) return;

        if (!isConnectedToWarehouse)
        {
            foreach (var need in trackedNeeds)
            {
                need.isMet = false;
            }
        }

        int needsMetCount = trackedNeeds.Count(n => n.isMet);

        maxResidents = baseResidents + (currentTier.needs.Count * residentsPerNeedMet);
        currentResidents = baseResidents + (needsMetCount * residentsPerNeedMet);
        currentResidents = Mathf.Min(currentResidents, maxResidents);

        currentHappiness = 10 + (needsMetCount * 2) - ((currentTier.needs.Count - needsMetCount) * 1);
        currentHappiness = Mathf.Clamp(currentHappiness, 0, 20);

        PopulationManager.Instance.UpdatePopulationForHouse(this, currentResidents);
    }

    // ▼▼▼【错误来源4】您的代码里缺少这个方法 ▼▼▼
    public Vector3Int GetGridPosition()
    {
        return _myGridPosition;
    }

    public bool CanUpgrade()
    {
        if (currentTier.nextTier == null) return false;
        if (currentResidents < maxResidents) return false;

        foreach (var material in currentTier.upgradeMaterials)
        {
            if (ResourceManager.Instance.GetWarehouseStock(material.item) < material.amount) return false;
        }
        return true;
    }

    public void TryToUpgrade()
    {
        if (!CanUpgrade())
        {
            Debug.Log("升级条件未满足！");
            return;
        }

        foreach (var material in currentTier.upgradeMaterials)
        {
            ResourceManager.Instance.TryConsumeWarehouseItem(material.item, material.amount);
        }

        PopulationManager.Instance.UnregisterHouse(this);
        currentTier = currentTier.nextTier;
        PopulationManager.Instance.RegisterHouse(this);
        InitializeNeeds();
        RecalculateState();

        Debug.Log($"<color=cyan>房屋升级成功！现在是 {currentTier.tierName}！</color>");
    }
}