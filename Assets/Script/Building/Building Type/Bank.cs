// Bank.cs - Final Version with Interface
using UnityEngine;

public class Bank : MonoBehaviour, IActivatableBuilding // Implements the interface
{
    public void Activate(Vector3Int gridPosition)
    {
        this.enabled = true;
    }

    void OnEnable()
    {
        ResourceManager.Instance?.RegisterBank();
    }

    void OnDisable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.UnregisterBank();
        }
    }
}