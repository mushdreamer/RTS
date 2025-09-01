// Institute.cs - Final Version with Interface
using UnityEngine;

public class Institute : MonoBehaviour, IActivatableBuilding // Implements the interface
{
    public void Activate(Vector3Int gridPosition)
    {
        this.enabled = true;
        Debug.Log("Institute has been activated.");
    }
}