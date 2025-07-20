// BuildingConnector.cs - 升级版
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // 需要引入LINQ

public class BuildingConnector : MonoBehaviour
{
    public static BuildingConnector Instance { get; private set; }

    [Tooltip("需要引用场景中的PlacementSystem来获取GridData")]
    public PlacementSystem placementSystem;

    private GridData floorData;

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

    void Start()
    {
        if (placementSystem != null)
        {
            floorData = placementSystem.GetFloorData();
        }
    }

    /// <summary>
    /// 【核心通用方法】检查一个起点建筑是否通过道路网络连接到任意一个指定类型的目标建筑。
    /// </summary>
    /// <param name="startBuildingPos">起点建筑的任意一个网格坐标</param>
    /// <param name="targetType">要寻找的目标建筑类型 (例如 BuildingType.Warehouse)</param>
    /// <returns>如果连接则返回true，否则返回false</returns>
    public bool CheckConnection(Vector3Int startBuildingPos, BuildingType targetType)
    {
        if (floorData == null)
        {
            return false;
        }

        // ▼▼▼【核心修改点：从这里开始】▼▼▼

        // 1. 根据起点位置，获取建筑的完整占地信息
        PlacementData placementData = floorData.GetPlacementDataAt(startBuildingPos);
        if (placementData == null)
        {
            return false;
        }
        List<Vector3Int> occupiedPositions = placementData.occupiedPositions;

        Queue<Vector3Int> positionsToVisit = new Queue<Vector3Int>();
        HashSet<Vector3Int> visitedPositions = new HashSet<Vector3Int>();

        // 2. 遍历建筑占据的每一个格子，检查它们所有的邻居，来找到寻路的起点
        foreach (var occupiedPos in occupiedPositions)
        {
            // 将建筑自己占据的所有格子先加入“已访问”列表，避免回头路
            visitedPositions.Add(occupiedPos);

            // 检查这个格子的四个方向的邻居
            foreach (var offset in new Vector3Int[] { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right })
            {
                Vector3Int neighborPos = occupiedPos + offset;

                // 如果这个邻居是道路，并且我们还没访问过它，就把它作为寻路起点
                if (floorData.GetObjectTypeAt(neighborPos) == BuildingType.Road && !visitedPositions.Contains(neighborPos))
                {
                    positionsToVisit.Enqueue(neighborPos);
                    // 标记为已访问，防止重复添加
                    visitedPositions.Add(neighborPos);
                }
            }
        }

        // ▼▼▼【核心修改点：到这里结束】▲▲▲

        if (positionsToVisit.Count == 0)
        {
            Debug.LogWarning("Pathfinding Failed");
            return false;
        }

        // 后续的广度优先搜索逻辑完全保持不变
        while (positionsToVisit.Count > 0)
        {
            Vector3Int currentPos = positionsToVisit.Dequeue();
            foreach (var offset in new Vector3Int[] { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right })
            {
                Vector3Int neighborPos = currentPos + offset;
                if (visitedPositions.Contains(neighborPos)) continue;

                BuildingType neighborType = floorData.GetObjectTypeAt(neighborPos);

                if (neighborType == targetType)
                {
                    Debug.Log($"<color=cyan>Succeed！We find target at {neighborPos} {targetType}！</color>");
                    return true;
                }

                if (neighborType == BuildingType.Road)
                {
                    visitedPositions.Add(neighborPos);
                    positionsToVisit.Enqueue(neighborPos);
                }
            }
        }

        Debug.LogWarning("Pathfinding end, we don't find any path");
        return false;
    }
}