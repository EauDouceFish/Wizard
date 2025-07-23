using UnityEngine;
[CreateAssetMenu(fileName = "BuffData", menuName = "BuffSystem/BuffData", order = 0)]
public class BuffData : ScriptableObject
{
    [Header("����ʵ�ֹ���")]
    [InspectorName("���ȼ�")]
    [Tooltip("buff�����ȼ����ѵ�������")]
    public int maxLevel = 1;

    [InspectorName("�������Ե�")]
    [Tooltip("buff�ǲ��������Եģ����������ʱ���Գ���ʱ�䡣")]
    public bool isPermanent = false;

    [InspectorName("������ʱ��")]
    [Tooltip("Buff��������ʱ��,ͬʱҲ�ǳ�ʼ����ʱ�䡣")]
    public float maxDuration = 0.02f;

    [InspectorName("��ʱ������")]
    [Tooltip("��buff����ʱ�䵽0ʱҪ���͵ĵȼ����������Ϊ0�������0����")]
    public int demotion = 1;

    [InspectorName("�Զ�ˢ��")]
    [Tooltip("��buff�ĵȼ�����ʱ��ﵽ���ȼ����������ȼ�ʱ���Ƿ��Զ�ˢ�³���ʱ�䡣")]
    public bool autoRefresh = true;

    [InspectorName("��ͻ����")]
    [Tooltip("��������ͬ��Դ����ͬһ����λʩ��ͬһ��buffʱ�ĳ�ͻ�������BuffConflictResolution")]
    public BuffConflictResolution buffConflictResolution = BuffConflictResolution.Combine;


    [Header("���ڷ��ദ��")]
    [InspectorName("Buff���ࣺ�û�����")]
    [Tooltip("��buff���з��ദ������/����/����Buff")]
    public BuffType buffType = BuffType.Neutral;

    [InspectorName("Buff��ϡ�ж�")]
    [Tooltip("Buff��ϡ�ж�")]
    public BuffRarity buffRarity = BuffRarity.Common;

    [InspectorName("��ǩ")]
    [Tooltip("���������ڶ�buff���ж�������Tag���ദ��")]
    public string[] tags = null;

    [Header("����UI��ʾ")]
    [InspectorName("����")]
    public string buffName = "δ֪��BUff";

    [InspectorName("Buff ����")]
    public string description = "δ֪������";


    [InspectorName("Icon·��")]
    public string iconPath = "Icon\\";
    private void OnValidate()
    {
        if (maxLevel <= 0)
        {
            Debug.LogError("buff�����ȼ�����С�ڻ����0");
            maxLevel = 1;
        }
        if (maxDuration <= 0)
        {
            Debug.LogError("buff��������ʱ�䲻��С�ڻ����0");
            maxDuration = 0.02f;
        }
        if (demotion < 0)
        {
            Debug.LogError("buff�ĵ�ʱ����������С��0");
            demotion = 0;
        }
    }
}