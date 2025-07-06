// UnitSelectionManager.cs - ������������קѡ����
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; set; }

    // --- ���� ---
    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> selectedObjects = new List<GameObject>();

    // --- LayerMasks ---
    [Header("Layers")]
    public LayerMask clickable;
    public LayerMask constructable;
    public LayerMask ground;
    public LayerMask attackable;

    // <<< ����������ԭ�еĴ����мӻ�������� >>>
    [HideInInspector]
    public bool playedDuringThisDrag = false;

    // --- ���� ---
    public GameObject groundMarker;
    private Camera cam;

    // --- �¼� ---
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
            // <<< ������UI�����͸��� >>>
            // �����ǰ���ָ�����������κ�UI��Ϸ�����ϣ���ֱ�ӷ��أ���ִ�к�������Ϸ����ѡ���߼�
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // --- ������ԭ�е�ѡ���߼������ֲ��� ---
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

        // ר��Ϊ��λ�����Ӿ�Ч��
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

    // <<< ����������ԭ�еĴ����мӻ� DragSelect ���� >>>
    /// <summary>
    /// ������ק��ѡ��λ
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
            // �����ﲥ����Ч����ʹ�� playedDuringThisDrag ��־
            if (isVisible && !playedDuringThisDrag)
            {
                // SoundManager.Instance.PlayUnitSelectionSound(); // ����������Ч������
                playedDuringThisDrag = true;
            }
            indicator.gameObject.SetActive(isVisible);
        }
    }

    // ... ������ԭ�еķ��� ...
}