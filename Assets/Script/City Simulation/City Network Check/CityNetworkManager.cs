// CityNetworkManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CityNetworkManager : MonoBehaviour
{
    public static CityNetworkManager Instance { get; private set; }

    public PlacementSystem placementSystem;

    // 记录每个网格坐标属于哪个网络ID
    private Dictionary<Vector3Int, int> _networkMap = new Dictionary<Vector3Int, int>();
    // 记录每个网络ID下，各阶层人口的总数
    private Dictionary<int, Dictionary<PopulationTier, int>> _populationByNetwork = new Dictionary<int, Dictionary<PopulationTier, int>>();

    private int _nextNetworkId = 1;
    private GridData _floorData;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    void Start()
    {
        _floorData = placementSystem.GetFloorData();
        // 游戏开始时，以及之后每5秒，重新扫描和计算一次所有网络
        InvokeRepeating(nameof(RecalculateAllNetworks), 1f, 5f);
    }

    /// <summary>
    /// 核心方法：重新计算整个地图的所有网络
    /// </summary>
    public void RecalculateAllNetworks()
    {
        // 重置所有数据
        _networkMap.Clear();
        _populationByNetwork.Clear();
        _nextNetworkId = 1;

        // 获取所有已放置对象的位置
        var allPlacedPositions = _floorData.GetAllOccupiedPositions();

        foreach (var pos in allPlacedPositions)
        {
            // 如果这个位置已经被分配过网络ID，说明它属于一个已发现的网络，跳过
            if (_networkMap.ContainsKey(pos)) continue;

            // 否则，从这个位置开始进行“洪水填充”算法，发现一个新网络
            FloodFillNetwork(pos, _nextNetworkId);
            _nextNetworkId++;
        }
    }

    /// <summary>
    /// 洪水填充算法，用于发现并标记一个完整连接的网络
    /// </summary>
    private void FloodFillNetwork(Vector3Int startPos, int networkId)
    {
        Queue<Vector3Int> toVisit = new Queue<Vector3Int>();
        toVisit.Enqueue(startPos);

        if (_networkMap.ContainsKey(startPos)) return; // 已被访问，则退出

        _networkMap[startPos] = networkId;

        while (toVisit.Count > 0)
        {
            Vector3Int currentPos = toVisit.Dequeue();
            BuildingType currentType = _floorData.GetObjectTypeAt(currentPos);

            // 如果当前建筑是房屋，则为它所属的网络增加人口
            if (currentType == BuildingType.House)
            {
                AddPopulationToNetwork(networkId, _floorData.GetHousePopulationData(currentPos));
            }

            // 检查邻居
            foreach (var offset in new Vector3Int[] { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right })
            {
                Vector3Int neighborPos = currentPos + offset;
                if (_floorData.GetObjectTypeAt(neighborPos) != BuildingType.None && !_networkMap.ContainsKey(neighborPos))
                {
                    _networkMap[neighborPos] = networkId;
                    toVisit.Enqueue(neighborPos);
                }
            }
        }
    }

    // 辅助方法，用于为网络增加人口
    private void AddPopulationToNetwork(int networkId, (PopulationTier tier, int count) popData)
    {
        if (!_populationByNetwork.ContainsKey(networkId))
        {
            _populationByNetwork[networkId] = new Dictionary<PopulationTier, int>();
        }
        if (!_populationByNetwork[networkId].ContainsKey(popData.tier))
        {
            _populationByNetwork[networkId][popData.tier] = 0;
        }
        _populationByNetwork[networkId][popData.tier] += popData.count;
    }

    // --- 公共接口 ---
    public int GetNetworkIdAt(Vector3Int position)
    {
        return _networkMap.GetValueOrDefault(position, -1); // -1 代表不属于任何网络
    }

    public int GetAvailablePopulationOnNetwork(int networkId, PopulationTier tier)
    {
        if (_populationByNetwork.ContainsKey(networkId) && _populationByNetwork[networkId].ContainsKey(tier))
        {
            return _populationByNetwork[networkId][tier];
        }
        return 0;
    }
}