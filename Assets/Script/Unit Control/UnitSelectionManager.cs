// UnitSelectionManager.cs - 重新整合了拖拽选择功能
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; set; }

    // --- 数据 ---
    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> selectedObjects = new List<GameObject>();

    // --- LayerMasks ---
    [Header("Layers")]
    public LayerMask clickable;
    public LayerMask constructable;
    public LayerMask ground;
    public LayerMask attackable;

    // <<< 新增：从你原有的代码中加回这个变量 >>>
    [HideInInspector]
    public bool playedDuringThisDrag = false;

    // --- 引用 ---
    public GameObject groundMarker;
    private Camera cam;

    // --- 事件 ---
    public event Action<GameObject> OnSelectionChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // <<< 新增：UI点击穿透检查 >>>
            // 如果当前鼠标指针正悬浮在任何UI游戏对象上，则直接返回，不执行后续的游戏世界选择逻辑
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // --- 以下是原有的选择逻辑，保持不变 ---
            LayerMask selectionMask = clickable | constructable;
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectionMask))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    // MultiSelect(hit.collider.gameObject);
                }
                else
                {
                    SelectObject(hit.collider.gameObject);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    DeselectAll();
                }
            }
        }
    }

    public void SelectObject(GameObject obj)
    {
        DeselectAll();
        if (obj != null) { selectedObjects.Add(obj); }

        // 专门为单位触发视觉效果
        if (obj != null && obj.TryGetComponent<UnitMovement>(out _))
        {
            TriggerSelectionIndicator(obj, true);
        }

        OnSelectionChanged?.Invoke(obj);
    }

    public void DeselectAll()
    {
        foreach (var obj in selectedObjects)
        {
            if (obj != null && obj.TryGetComponent<UnitMovement>(out _))
            {
                TriggerSelectionIndicator(obj, false);
            }
        }
        selectedObjects.Clear();
        OnSelectionChanged?.Invoke(null);
    }

    // <<< 新增：从你原有的代码中加回 DragSelect 方法 >>>
    /// <summary>
    /// 用于拖拽框选单位
    /// </summary>
    internal void DragSelect(GameObject unit)
    {
        if (selectedObjects.Contains(unit) == false)
        {
            selectedObjects.Add(unit);
            TriggerSelectionIndicator(unit, true);
        }
    }

    private void TriggerSelectionIndicator(GameObject unit, bool isVisible)
    {
        if (unit == null) return;
        Transform indicator = unit.transform.Find("Indicator");
        if (indicator != null)
        {
            // 在这里播放音效，并使用 playedDuringThisDrag 标志
            if (isVisible && !playedDuringThisDrag)
            {
                // SoundManager.Instance.PlayUnitSelectionSound(); // 假设你有音效管理器
                playedDuringThisDrag = true;
            }
            indicator.gameObject.SetActive(isVisible);
        }
    }

    // ... 其他你原有的方法 ...
}