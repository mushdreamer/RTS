// In folder: Assets/Scripts/PopulationTier.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Population Tier", menuName = "Item and Population Tier/Population Tier")]
public class PopulationTier : ScriptableObject
{
    [Header("�ײ���Ϣ")]
    public string tierName;
    public int residentsPerHouse;

    [Header("�����б�")]
    public List<Need> needs;

    [Header("�����߼�")]
    public PopulationTier nextTier; // <<< �������������Ŀ��ײ�
    public List<BuildRequirement> upgradeMaterials; // <<< ������������Ҫ�Ĳ���

    // �������������һ���򵥵����ԣ���������������Ҫ���Ҹ�����ֵ
    public int HappinessToUpgrade => 11; // ��ζ����Ҫ���Ҹ��Ȳ�������
}

// BuildRequirement ������֮ǰ�Ѿ�������ˣ����ڿ��Ը�������
// ������Ҫȷ������ĳ���ط��ǿɷ��ʵģ����� ObjectsDatabseSO.cs ��
// ������� 'BuildRequirement' not found �Ĵ���
// ���ǿ��԰����Ķ���� ObjectsDatabseSO.cs �Ƶ�һ�������� BuildRequirement.cs �ļ��