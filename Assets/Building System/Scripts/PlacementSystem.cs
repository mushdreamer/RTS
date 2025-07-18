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