// TransmissionTower.cs - Final Version with Interface
using UnityEngine;
using System.Linq;

public class TransmissionTower : MonoBehaviour, IEnergyConsumer, IActivatableBuilding // Implements the interface
{
    [Header("电网属性")]
    public float powerRequirement = 1f;

    [Header("范围与容量")]
    public float coverageRadius = 20f;
    public int maxHouseConnections = 10;

    private bool isConnectedToPlant = false;
    public float RequestedPower => isConnectedToPlant ? powerRequirement : 0;
    public bool IsPowered { get; private set; }

    // This is the required method from the IActivatableBuilding interface
    public void Activate(Vector3Int gridPosition)
    {
        // Enable the component to start its FixedUpdate loop and register with manager
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

    void Start()
    {
        CheckConnectionToPowerPlant();
    }

    void FixedUpdate()
    {
        CheckConnectionToPowerPlant();
    }

    private void CheckConnectionToPowerPlant()
    {
        var colliders = Physics.OverlapSphere(transform.position, 1f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);
        CoalPowerPlant closestValidPlant = colliders
            .Select(col => col.GetComponentInParent<CoalPowerPlant>())
            .Where(plant => plant != null && Vector3.Distance(transform.position, plant.transform.position) <= plant.powerRadius)
            .OrderBy(plant => Vector3.Distance(transform.position, plant.transform.position))
            .FirstOrDefault();
        isConnectedToPlant = (closestValidPlant != null);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsPowered ? Color.cyan : (isConnectedToPlant ? Color.blue : Color.gray);
        Gizmos.DrawWireSphere(transform.position, coverageRadius);
    }
}