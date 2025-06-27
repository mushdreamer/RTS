using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsDatabseSO : ScriptableObject
{
    public List<ObjectData> objectsData;


    public ObjectData GetObjectByID(int id)
    {
        foreach (ObjectData obj in objectsData)
        {
            if (obj.ID == id)
            {
                return obj;
            }
        }

        return new(); // This cannot happen
    }

}

public enum BuildingType
{
    None,
    CommandCenter,
    PowerPlant
}

[System.Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public BuildingType thisBuildingType { get; private set; }

    [field: SerializeField]
    [TextArea(3, 10)]
    public string description;

    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;

    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    [field: SerializeField]
    public int creditCost { get; private set; } // 1. 明确的金钱成本

    [field: SerializeField]
    public List<BuildRequirement> materialRequirements { get; private set; } // 2. 将原来的resourceRequirements改名为materialRequirements

    [field: SerializeField]
    public List<BuildingType> buildDependency { get; private set; }

    [field: SerializeField]
    public List<BuildBenefits> benefits { get; private set; }

  
}

[System.Serializable]
public class BuildRequirement
{
    public ItemData item; // 3. 不再使用枚举，而是直接引用ItemData
    public int amount;
}


[System.Serializable]
public class BuildBenefits
{
    public enum BenefitType
    {
        Housing
    }


    public string benefit;
    public Sprite benefitIcon;
    public BenefitType benefitType;
    public int benefitAmount;
}