// House.cs - 增加了幸福度逻辑
using UnityEngine;

public class House : MonoBehaviour
{
    public PopulationTier currentTier;

    [Header("状态")]
    [Range(0, 20)] // 将幸福度限制在0到20的范围内
    public int currentHappiness = 10; // 居民的基础幸福度

    private int _residentCount;
    private float _consumptionIntervalInSeconds = 60f;
    private bool _isActivated = false;

    void Start() { }

    public void ActivateHouse()
    {
        if (_isActivated) return;
        if (currentTier == null)
        {
            Debug.LogError("无法激活房屋，因为 currentTier 为空!", this.gameObject);
            return;
        }

        _isActivated = true;
        _residentCount = currentTier.residentsPerHouse;

        // <<< 将房屋注册到管理器中 >>>
        PopulationManager.Instance.RegisterHouse(this);

        PopulationManager.Instance.UpdatePopulation(currentTier, _residentCount);
        InvokeRepeating(nameof(ConsumeNeeds), _consumptionIntervalInSeconds, _consumptionIntervalInSeconds);
        Debug.Log($"房屋 {this.gameObject.name} 已被激活，人口 +{_residentCount}");
    }

    void OnDestroy()
    {
        if (_isActivated && PopulationManager.Instance != null)
        {
            // <<< 从管理器中注销房屋 >>>
            PopulationManager.Instance.UnregisterHouse(this);
            PopulationManager.Instance.UpdatePopulation(currentTier, -_residentCount);
            Debug.Log($"已激活的房屋 {this.gameObject.name} 已被摧毁，人口 -{_residentCount}");
        }
    }

    private void ConsumeNeeds()
    {
        if (currentTier == null) return;

        foreach (var need in currentTier.needs)
        {
            bool consumedSuccessfully = ResourceManager.Instance.TryConsumeWarehouseItem(need.item, need.consumptionPerMinute);

            if (consumedSuccessfully)
            {
                // <<< 需求满足，增加幸福度 >>>
                // 我们暂时设定每次满足需求都+1幸福度
                currentHappiness++;
            }
            else
            {
                // <<< 需求未满足，降低幸福度 >>>
                // 每次未满足需求都-2幸福度，惩罚更重一些
                currentHappiness -= 2;
            }
        }

        // <<< 使用 Mathf.Clamp 确保幸福度不会超出范围 >>>
        currentHappiness = Mathf.Clamp(currentHappiness, 0, 20);
    }
}