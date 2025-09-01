// Factory.cs - Final Version with Interface
using UnityEngine;

public class Factory : MonoBehaviour, IEnergyConsumer, IActivatableBuilding // Implements the interface
{
    [Tooltip("工厂运转需要的电量")]
    public float powerRequirement = 15f;

    public float RequestedPower => powerRequirement;
    public bool IsPowered { get; private set; }

    public void Activate(Vector3Int gridPosition)
    {
        this.enabled = true;
    }

    public void SupplyPower(float suppliedAmount)
    {
        IsPowered = (suppliedAmount >= powerRequirement);
    }

    void OnEnable()
    {
        EnergyGridManager.Instance?.RegisterConsumer(this);
    }

    void OnDisable()
    {
        EnergyGridManager.Instance?.UnregisterConsumer(this);
    }
}