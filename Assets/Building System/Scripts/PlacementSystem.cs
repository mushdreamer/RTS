// PlacementSystem.cs - 修正版
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
    [SerializeField] private GridData floorData, furnitureData;
    [SerializeField] private PreviewSystem previewSystem;
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    [SerializeField] private ObjectPlacer objectPlacer;
    int selectedID;
    IBuildingState buildingState;
    public bool inSellMode;

    // ▼▼▼【核心修改】▼▼▼
    // 将 Start() 方法改为 Awake()
    // 这样可以确保 floorData 在其他任何脚本的 Start() 方法运行前被创建
    private void Awake()
    {
        floorData = new();
        furnitureData = new();
        floorData.InitializeWithDatabase(database);
        furnitureData.InitializeWithDatabase(database);
    }

    public void StartPlacement(int ID)
    {
        selectedID = ID;
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

    // ▼▼▼ 【新增代码】 ▼▼▼
    /// <summary>
    /// 根据世界坐标清除一个建筑的占位数据
    /// </summary>
    /// <param name="position">建筑所在的世界坐标</param>
    public void ClearBuildingDataAt(Vector3 position)
    {
        Vector3Int gridPosition = grid.WorldToCell(position);

        // 我们需要检查两个数据层，因为我们不确定建筑在哪一层
        // 通常建筑都在 floorData，但这样做更健壮
        if (furnitureData.GetObjectTypeAt(gridPosition) != BuildingType.None)
        {
            furnitureData.RemoveObjectAt(gridPosition);
        }

        if (floorData.GetObjectTypeAt(gridPosition) != BuildingType.None)
        {
            floorData.RemoveObjectAt(gridPosition);
        }
    }
    // ▲▲▲ 【新增代码结束】 ▲▲▲


    private void EndSelling()
    {
        inSellMode = false;
    }

    private void PlaceStructure()
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        buildingState.OnAction(gridPosition);
        ObjectData ob = database.GetObjectByID(selectedID);
        foreach (BuildBenefits bf in ob.benefits)
        {
            CalculateAndAddBenefit(bf);
        }
        StopPlacement();
    }

    private void CalculateAndAddBenefit(BuildBenefits bf)
    {
        switch (bf.benefitType)
        {
            case BuildBenefits.BenefitType.Housing:
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

    // 这个方法之前被 BuildingConnector 调用
    public GridData GetFloorData()
    {
        return floorData;
    }

    // 注意：之前添加在这里的 IsConnectedToWarehouse 方法应该被删除，因为它已经被移到 BuildingConnector 里了
}