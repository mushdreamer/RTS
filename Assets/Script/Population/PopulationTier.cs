// In folder: Assets/Scripts/PopulationTier.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Population Tier", menuName = "Item and Population Tier/Population Tier")]
public class PopulationTier : ScriptableObject
{
    [Header("�ײ���Ϣ")]
    public string tierName; // ���� "ũ��"
    public int residentsPerHouse; // ÿ���������ɵľ�������

    [Header("�����б�")]
    public List<Need> needs; // �ýײ����������

    [Header("�����߼�")]
    public PopulationTier nextTier; // �������Ŀ��ײ� (���ڿ�������)
    public List<ItemData> upgradeMaterials; // ������Ҫ�Ĳ��� (���ڿ�������)
}
