// GridData.cs - 完整最终版
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridData
{
    private Dictionary<Vector3Int, PlacementData> placedObjects = new Dictionary<Vector3Int, PlacementData>();
    private ObjectsDatabseSO database;

    public void InitializeWithDatabase(ObjectsDatabseSO database)
    {
        this.database = database;
    }

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int id, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, id, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new List<Vector3Int>();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos)) return false;
        }
        return true;
    }

    public BuildingType GetObjectTypeAt(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition)) return BuildingType.None;
        int objectID = placedObjects[gridPosition].ID;
        ObjectData objectData = database.GetObjectByID(objectID);
        return objectData?.thisBuildingType ?? BuildingType.None;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition))
        {
            List<Vector3Int> positionsToRemove = new List<Vector3Int>(placedObjects[gridPosition].occupiedPositions);
            foreach (var pos in positionsToRemove)
            {
                placedObjects.Remove(pos);
            }
        }
    }

    public int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition)) return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    public PlacementData GetPlacementDataAt(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition))
        {
            return placedObjects[gridPosition];
        }
        return null;
    }

    public List<Vector3Int> GetAllOccupiedPositions()
    {
        return new List<Vector3Int>(placedObjects.Keys);
    }

    public (PopulationTier tier, int count) GetHousePopulationData(Vector3Int position)
    {
        int index = GetRepresentationIndex(position);
        if (index == -1 || ObjectPlacer.Instance == null) return (null, 0);

        GameObject obj = ObjectPlacer.Instance.GetObjectByIndex(index);
        if (obj != null && obj.TryGetComponent<House>(out var house))
        {
            return (house.currentTier, house.currentResidents);
        }
        return (null, 0);
    }
}

// ▼▼▼【解决问题的关键】▼▼▼
// 这个 PlacementData 类的定义必须存在于文件的末尾。
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = id;
        PlacedObjectIndex = placedObjectIndex;
    }
}