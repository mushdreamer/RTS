// NeedDisplayItem.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NeedDisplayItem : MonoBehaviour
{
    [Header("UI元素")]
    public Image needIcon;
    public TextMeshProUGUI needNameText;
    public GameObject checkmarkObject; // 用于显示“已满足”的对勾图标

    /// <summary>
    /// 设置并显示此条需求的状态
    /// </summary>
    /// <param name="need">需求的数据来源 (HouseNeedState)</param>
    public void Setup(HouseNeedState needState)
    {
        if (needState == null || needState.associatedNeed == null) return;

        // 从关联的ItemData获取图标和名称
        ItemData item = needState.associatedNeed.item;
        needIcon.sprite = item.icon;
        needNameText.text = item.itemName;

        // 根据是否满足来决定是否显示对勾
        checkmarkObject.SetActive(needState.isMet);
    }
}