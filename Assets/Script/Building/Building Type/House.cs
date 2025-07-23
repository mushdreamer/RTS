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

    // House.cs - 修改后
    public void RecalculateState()
    {
        if (currentTier == null) return;

        // ▼▼▼【核心修改点：从这里开始】▼▼▼

        // 首先，判断是否连接到仓库
        if (isConnectedToWarehouse)
        {
            // 如果已连接，执行我们之前所有的逻辑
            int needsMetCount = trackedNeeds.Count(n => n.isMet);

            maxResidents = baseResidents + (currentTier.needs.Count * residentsPerNeedMet);
            currentResidents = baseResidents + (needsMetCount * residentsPerNeedMet);
            currentResidents = Mathf.Min(currentResidents, maxResidents);

            currentHappiness = 10 + (needsMetCount * 2) - ((currentTier.needs.Count - needsMetCount) * 1);
            currentHappiness = Mathf.Clamp(currentHappiness, 0, 20);
        }
        else
        {
            // 如果未连接，则不满足任何需求，且没有任何居民和幸福度
            foreach (var need in trackedNeeds)
            {
                need.isMet = false;
            }

            maxResidents = baseResidents + (currentTier.needs.Count * residentsPerNeedMet); // 最大潜力仍然可以显示
            currentResidents = 0; // 关键！当前居民为0
            currentHappiness = 0; // 没有居民就没有幸福度
        }

        // ▲▲▲【核心修改点：到这里结束】▲▲▲

        // 最终，无论上面哪种情况，都将计算出的（可能是0）人口数量上报
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
            Debug.Log("Upgrade Condition doesn't Fulfill！");
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

        Debug.Log($"<color=cyan>House upgrade succeed！</color>");
    }
}