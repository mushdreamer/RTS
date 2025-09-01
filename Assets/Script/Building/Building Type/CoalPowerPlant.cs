// CoalPowerPlant.cs - Final Version with Interface
using UnityEngine;

public class CoalPowerPlant : MonoBehaviour, IEnergyProducer, IActivatableBuilding // Implements the interface
{
    [Header("电网属性")]
    public float maxProduction = 100f;

    [Header("范围与容量")]
    public float powerRadius = 30f;
    public int maxTowerConnections = 5;

    [Header("环境影响")]
    public float co2EmissionPerSecond = 2f;

    private bool isOperating = false;
    public float CurrentProduction => isOperating ? maxProduction : 0;

    // This is the required method from the IActivatableBuilding interface
    public void Activate(Vector3Int gridPosition)
    {
        isOperating = true;
        // Enable the component to start its Update loop and register with manager
        this.enabled = true;
        Debug.Log($"{gameObject.name} has been activated and is now operating.");
    }

    void OnEnable()
    {
        EnergyGridManager.Instance?.RegisterProducer(this);
    }

    void OnDisable()
    {
        EnergyGridManager.Instance?.UnregisterProducer(this);
    }

    void Update()
    {
        if (isOperating)
        {
            if (AirQualityManager.Instance != null)
            {
                AirQualityManager.Instance.AddCO2(co2EmissionPerSecond * Time.deltaTime);
            }
            else
            {
                Debug.LogError("Could not find an instance of AirQualityManager!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, powerRadius);
    }
}