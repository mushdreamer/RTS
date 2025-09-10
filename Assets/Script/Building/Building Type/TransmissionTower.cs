// TransmissionTower.cs - 最终修正版 (修复了索敌范围bug)
using UnityEngine;
using System.Linq;

public class TransmissionTower : MonoBehaviour, IEnergyConsumer, IActivatableBuilding
{
    [Header("电网属性")]
    public float powerRequirement = 1f;
    [Header("范围与容量")]
    public float coverageRadius = 20f;
    public int maxHouseConnections = 10;

    private bool isConnectedToPlant = false;
    public float RequestedPower => isConnectedToPlant ? powerRequirement : 0;
    public bool IsPowered { get; private set; }

    public void Activate(Vector3Int gridPosition) { this.enabled = true; }
    public void SupplyPower(float suppliedAmount) { IsPowered = (suppliedAmount >= powerRequirement); }

    void OnEnable() { EnergyGridManager.Instance?.RegisterConsumer(this); CheckConnectionToPowerPlant(); }
    void OnDisable() { EnergyGridManager.Instance?.UnregisterConsumer(this); }
    void FixedUpdate() { CheckConnectionToPowerPlant(); }

    private void CheckConnectionToPowerPlant()
    {
        // ▼▼▼【核心修正点】▼▼▼
        // 不再使用小范围的 OverlapSphere，而是查找场景中所有的电厂
        var allPlants = FindObjectsOfType<CoalPowerPlant>();
        bool isNowConnected = false;

        foreach (var plant in allPlants)
        {
            // 检查距离是否在电厂自身的供电半径内
            if (Vector3.Distance(this.transform.position, plant.transform.position) <= plant.powerRadius)
            {
                isNowConnected = true;
                break; // 只要找到一个供电的电厂即可
            }
        }
        isConnectedToPlant = isNowConnected;
        // ▲▲▲【修正结束】▲▲▲
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsPowered ? Color.cyan : (isConnectedToPlant ? Color.yellow : Color.gray);
        Gizmos.DrawWireSphere(transform.position, coverageRadius);
    }
}