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
    /// <summary>
    /// 【核心通用方法】检查一个起点建筑是否通过道路网络连接到任意一个指定类型的目标建筑。
    /// </summary>
    public bool CheckConnection(Vector3Int startBuildingPos, BuildingType targetType)
    {
        // ▼▼▼【核心修改点：从这里开始】▼▼▼
        // 在执行任何寻路逻辑之前，首先检查全局规则。
        if (CityRulesManager.Instance != null && !CityRulesManager.Instance.requireRoadsForConnection)
        {
            // 如果全局规则设置为“不需要道路连接”，则直接返回true，跳过所有寻路计算。
            // 这相当于给所有建筑一个“无线连接”的特权。
            return true;
        }
        // ▲▲▲【核心修改点：到这里结束】▲▲▲


        // ▼▼▼ 如果规则要求检查连接，则继续执行下面所有的原有寻路逻辑 ▼▼▼

        if (floorData == null)
        {
            return false;
        }

        PlacementData placementData = floorData.GetPlacementDataAt(startBuildingPos);
        if (placementData == null)
        {
            return false;
        }
        List<Vector3Int> occupiedPositions = placementData.occupiedPositions;

        Queue<Vector3Int> positionsToVisit = new Queue<Vector3Int>();
        HashSet<Vector3Int> visitedPositions = new HashSet<Vector3Int>();

        foreach (var occupiedPos in occupiedPositions)
        {
            visitedPositions.Add(occupiedPos);
            foreach (var offset in new Vector3Int[] { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right })
            {
                Vector3Int neighborPos = occupiedPos + offset;
                if (floorData.GetObjectTypeAt(neighborPos) == BuildingType.Road && !visitedPositions.Contains(neighborPos))
                {
                    positionsToVisit.Enqueue(neighborPos);
                    visitedPositions.Add(neighborPos);
                }
            }
        }

        if (positionsToVisit.Count == 0)
        {
            return false;
        }

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
                    return true;
                }
                if (neighborType == BuildingType.Road)
                {
                    visitedPositions.Add(neighborPos);
                    positionsToVisit.Enqueue(neighborPos);
                }
            }
        }

        return false;
    }
}