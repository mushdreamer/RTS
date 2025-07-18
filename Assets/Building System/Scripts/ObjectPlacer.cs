using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position, Vector3Int gridPosition) // <-- 新增了 gridPosition 参数
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        // 把世界坐标和网格坐标都传递下去
        newObject.GetComponent<Constructable>().ConstructableWasPlaced(position, gridPosition); // <-- 新增了 gridPosition 参数
        placedGameObjects.Add(newObject);
        return placedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if(placedGameObjects.Count <= gameObjectIndex 
            || placedGameObjects[gameObjectIndex] == null)
             return;
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
