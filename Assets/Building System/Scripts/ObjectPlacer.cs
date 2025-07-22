// ObjectPlacer.cs - 升级为单例
using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    // === 【新增】单例模式 ===
    public static ObjectPlacer Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    // === 单例模式结束 ===

    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position, Vector3Int gridPosition)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        newObject.GetComponent<Constructable>().ConstructableWasPlaced(position, gridPosition);
        placedGameObjects.Add(newObject);
        return placedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
            return;
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }

    // === 【新增】供 GridData 调用的方法 ===
    public GameObject GetObjectByIndex(int index)
    {
        if (index >= 0 && index < placedGameObjects.Count)
        {
            return placedGameObjects[index];
        }
        return null;
    }
}