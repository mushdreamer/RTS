using UnityEngine;
using System.Collections.Generic;

public class HouseNeedsUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Transform needsContainer;
    [SerializeField] private GameObject needDisplayPrefab;

    void Start()
    {
        if (mainPanel == null || needsContainer == null || needDisplayPrefab == null)
        {
            Debug.LogError("HouseNeedsUIController 的某些UI引用未在Inspector中设置！", this);
            return;
        }
        mainPanel.SetActive(false);
    }

    public void ShowNeedsForHouse(House selectedHouse)
    {
        Debug.Log($"[时刻 B - UIController] 准备为房屋创建UI。读取到列表中项目数量: {selectedHouse.trackedNeeds.Count}");

        mainPanel.SetActive(true);

        foreach (Transform child in needsContainer)
        {
            Destroy(child.gameObject);
        }

        if (selectedHouse == null || selectedHouse.trackedNeeds == null) return;

        foreach (HouseNeedState needState in selectedHouse.trackedNeeds)
        {
            Instantiate(needDisplayPrefab, needsContainer);
            // The new prefab will be automatically positioned by the Vertical Layout Group.
            // We still need to get its script to set up the icon, text, etc.
            // (Note: To be fully robust, you'd get the last child, but Instantiate works fine here)
            NeedDisplayItem needItemScript = needsContainer.GetChild(needsContainer.childCount - 1).GetComponent<NeedDisplayItem>();

            if (needItemScript != null)
            {
                needItemScript.Setup(needState);
            }
        }
    }

    public void HidePanel()
    {
        mainPanel.SetActive(false);
    }
}