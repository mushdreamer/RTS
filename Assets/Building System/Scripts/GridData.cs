using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    // 我们需要一个对数据库的引用来查询ID
    private ObjectsDatabseSO database;

    // 添加一个方法来设置这个引用
    public void InitializeWithDatabase(ObjectsDatabseSO database)
    {
        this.database = database;
    }

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int Id, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccuply = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccuply, Id, placedObjectIndex);
        foreach (var pos in positionToOccuply)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception("Dictionary already contains this cell position");
            }
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal1 = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal1.Add(gridPosition + new Vector3Int(x,0,y));
            }
        }
        return returnVal1;
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

    // <<< 在这里添加新的核心函数 >>>
    /// <summary>
    /// 获取指定网格位置上对象的建筑类型 (BuildingType)。
    /// </summary>
    /// <param name="gridPosition">要查询的网格坐标</param>
    /// <returns>返回对象的BuildingType，如果该位置为空则返回BuildingType.None</returns>
    public BuildingType GetObjectTypeAt(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition))
        {
            return BuildingType.None;
        }

        // 从网格数据中获取对象的ID
        int objectID = placedObjects[gridPosition].ID;

        // 使用数据库根据ID查找对象的完整数据
        ObjectData objectData = database.GetObjectByID(objectID);

        if (objectData != null)
        {
            return objectData.thisBuildingType;
        }

        return BuildingType.None;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    internal PlacementData GetPlacementDataAt(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition))
        {
            return placedObjects[gridPosition];
        }
        return null;
    }

    // <<< 在文件末尾添加这个新的调试函数 >>>
    public void Debug_PrintAllOccupiedCells()
    {
        if (placedObjects.Count == 0)
        {
            Debug.Log("<color=orange>GridData DEBUG: 'placedObjects' 字典是空的。</color>");
            return;
        }

        Debug.Log($"<color=orange>GridData DEBUG: 'placedObjects' 字典包含 {placedObjects.Count} 个条目。</color>");
        foreach (var entry in placedObjects)
        {
            // entry.Key 是网格坐标 (Vector3Int)
            // entry.Value 是放置数据 (PlacementData)
            ObjectData objectData = database.GetObjectByID(entry.Value.ID);
            Debug.Log($"<color=orange>  - 坐标 {entry.Key} 被对象 '{objectData.Name}' (ID: {objectData.ID}) 占据。</color>");
        }
    }
}


public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}