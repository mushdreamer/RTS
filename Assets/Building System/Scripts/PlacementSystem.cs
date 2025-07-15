using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private ObjectsDatabseSO database;

    [SerializeField] private GridData floorData, furnitureData; // floor things like roads, furniture change to "buildings"

    [SerializeField] private PreviewSystem previewSystem;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;

    int selectedID;

    IBuildingState buildingState;

    public bool inSellMode;

    private void Start()
    {

        floorData = new();
        furnitureData = new();

        // <<< 在这里添加初始化代码 >>>
        floorData.InitializeWithDatabase(database);
        furnitureData.InitializeWithDatabase(database);
    }

    public void StartPlacement(int ID)
    {
        //Debug.Log("Should Start Placement");

        selectedID = ID;

        //Debug.Log("Placement ID: " + ID);


        StopPlacement();

        buildingState = new PlacementState(ID, grid, previewSystem, database, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void RemovePlacementData(Vector3 position)
    {
        floorData.RemoveObjectAt(grid.WorldToCell(position));
    }

    public void StartRemoving()
    {
        StopPlacement();

        buildingState = new RemovingState(grid, previewSystem, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;

        inputManager.OnClicked += EndSelling;
        inputManager.OnExit += EndSelling;
    }

    private void EndSelling()
    {
        inSellMode = false;
    }

    private void PlaceStructure()
    {
        /*if(inputManager.IsPointerOverUI()){
            Debug.Log("Pointer was over UI - Returned");
            return;
        }*/

        // When we click on a cell, we get the cell
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);   


        // ---- Using the ID remove used resources from resource manager ---- // 
        ObjectData ob = database.GetObjectByID(selectedID);
       // ResourceManager.Instance.RemoveResourcesBasedOnRequirements(ob, database);

        // ---- Add Buildable Benifits ---- // 
        foreach (BuildBenefits bf in ob.benefits)
        {
            CalculateAndAddBenefit(bf);
        }

        // ---- Stop the placement after every build ---- // 
        StopPlacement();
    }

    private void CalculateAndAddBenefit(BuildBenefits bf)
    {
        switch (bf.benefitType)
        {
            case BuildBenefits.BenefitType.Housing:
             //   StatusManager.Instance.IncreaseHousing(bf.benefitAmount);
                break;
        }
    }

    private void StopPlacement()
    {
        if (buildingState == null)
            return;
       
        buildingState.EndState();

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;

        buildingState = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            inSellMode = true;
            StartRemoving();
        }
        // We return because we did not selected an item to place (not in placement mode)
        // So there is no need to show cell indicator
        if (buildingState == null)
            return;
      
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }

    }

    /// <summary>
    /// 检查指定世界坐标处的建筑是否与任何道路相连。
    /// </summary>
    /// <param name="buildingWorldPosition">建筑的任意一个世界坐标点</param>
    /// <returns>如果与道路相连则返回 true，否则返回 false</returns>
    public bool IsBuildingConnectedToRoad(Vector3 buildingWorldPosition)
    {
        Debug.Log("============== 开始连接检查 ==============");

        // <<< 在这里调用新的调试函数 >>>
        floorData.Debug_PrintAllOccupiedCells();

        Debug.Log($"1. 接收到建筑世界坐标: {buildingWorldPosition}");

        // 1. 将建筑的世界坐标转换为网格坐标
        Vector3Int buildingGridPosition = grid.WorldToCell(buildingWorldPosition);
        Debug.Log($"2. 转换为网格坐标: {buildingGridPosition}");

        // 2. 从GridData获取该建筑的PlacementData
        GridData data = floorData;

        // 如果该位置没有对象信息，直接返回false
        if (data.GetRepresentationIndex(buildingGridPosition) == -1)
        {
            Debug.LogError($"错误: 在网格坐标 {buildingGridPosition} 找不到任何建筑数据! 检查对象是否正确放置到了 floorData 中。");
            return false;
        }

        PlacementData placementData = data.GetPlacementDataAt(buildingGridPosition);

        if (placementData == null)
        {
            Debug.LogError("错误: 成功获取到索引，但无法获取PlacementData。");
            return false;
        }
        Debug.Log($"3. 成功找到建筑数据，该建筑占据 {placementData.occupiedPositions.Count} 个单元格。");

        // 4. 定义邻居的相对方向
        Vector3Int[] neighborOffsets = new Vector3Int[]
        {
        new Vector3Int(0, 0, 1),  // North
        new Vector3Int(0, 0, -1), // South
        new Vector3Int(1, 0, 0),  // East
        new Vector3Int(-1, 0, 0)  // West
        };

        // 5. 遍历建筑占据的每一个单元格
        foreach (var position in placementData.occupiedPositions)
        {
            Debug.Log($"--- 正在检查建筑单元格 {position} 的邻居 ---");
            // 6. 检查该单元格的每一个邻居
            foreach (var offset in neighborOffsets)
            {
                Vector3Int neighborPosition = position + offset;

                // 7. 获取邻居的对象类型
                BuildingType neighborType = data.GetObjectTypeAt(neighborPosition);

                // 这是最重要的日志！
                Debug.Log($"正在检查邻居 {neighborPosition}... 发现的对象类型是: {neighborType}");

                // 8. 如果邻居是道路，则证明已连接
                if (neighborType == BuildingType.Road)
                {
                    Debug.Log($"<color=green>成功! 在 {neighborPosition} 找到了道路! 返回 true。</color>");
                    Debug.Log("============== 检查结束 ==============");
                    return true;
                }
            }
        }

        // 9. 如果遍历完所有邻居都没有找到道路
        Debug.LogWarning("检查完所有邻居，未发现任何道路。返回 false。");
        Debug.Log("============== 检查结束 ==============");
        return false;
    }

    // 这是一个辅助函数，因为你的GridData没有直接暴露获取PlacementData的方法
    private PlacementData GetPlacementDataFromGrid(Vector3Int gridPosition, GridData data)
    {
        // 这是一个变通方法。理想情况下，GridData应该有一个公共方法来返回PlacementData。
        // 由于没有，我们只能假设我们能通过一个私有字段的公共方法间接访问。
        // 但既然没有，我们可以暂时忽略这个细节，因为逻辑的重点在于`occupiedPositions`。
        // 实际上，我们需要修改GridData来暴露这个信息。
        // 让我们回到GridData.cs添加一个函数。
        return data.GetPlacementDataAt(gridPosition);
    }
}
