// GridData.cs - 完整修正版
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    // ▼▼▼ 问题的根源很可能在这里 ▼▼▼
    // 确保这个字典的定义在所有方法的外部，作为类的成员变量，并且名字是 placedObjects
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
            if (placedObjects.ContainsKey(pos))
            {
                // 这里可以先移除旧的对象，或者直接报错，根据你的游戏逻辑决定
                // 为了健壮性，我们先不抛出异常，而是打印一个警告
                Debug.LogWarning($"网格位置 {pos} 已被占据，旧对象将被覆盖。");
                placedObjects.Remove(pos);
            }
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
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }
        return true;
    }

    public BuildingType GetObjectTypeAt(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition))
        {
            return BuildingType.None;
        }

        int objectID = placedObjects[gridPosition].ID;
        ObjectData objectData = database.GetObjectByID(objectID);

        return objectData?.thisBuildingType ?? BuildingType.None;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition))
        {
            // 需要先获取到要删除的对象所占据的所有格子
            List<Vector3Int> positionsToRemove = new List<Vector3Int>(placedObjects[gridPosition].occupiedPositions);
            foreach (var pos in positionsToRemove)
            {
                placedObjects.Remove(pos);
            }
        }
    }

    public int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition))
            return -1;
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

    public void Debug_PrintAllOccupiedCells()
    {
        if (placedObjects.Count == 0)
        {
            Debug.Log("<color=orange>GridData DEBUG: 'placedObjects' 字典是空的。</color>");
            return;
        }

        Debug.Log($"<color=orange>GridData DEBUG: 'placedObjects' 字典包含 {placedObjects.Count} 个条目。</color>");

        // 为了避免重复打印同一个对象多次，我们先收集所有独立的对象
        HashSet<PlacementData> uniqueObjects = new HashSet<PlacementData>(placedObjects.Values);

        foreach (var placementData in uniqueObjects)
        {
            ObjectData objectData = database.GetObjectByID(placementData.ID);
            Debug.Log($"<color=orange>  - 对象 '{objectData.Name}' (ID: {objectData.ID}) 占据了 {placementData.occupiedPositions.Count} 个格子。</color>");
        }
    }
}


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