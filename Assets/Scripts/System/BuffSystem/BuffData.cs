using UnityEngine;
[CreateAssetMenu(fileName = "BuffData", menuName = "BuffSystem/BuffData", order = 0)]
public class BuffData : ScriptableObject
{
    [Header("用于实现功能")]
    [InspectorName("最大等级")]
    [Tooltip("buff的最大等级（堆叠层数）")]
    public int maxLevel = 1;

    [InspectorName("是永久性的")]
    [Tooltip("buff是不是永久性的，如果是则处理时忽略持续时间。")]
    public bool isPermanent = false;

    [InspectorName("最大持续时间")]
    [Tooltip("Buff的最大持续时间,同时也是初始持续时间。")]
    public float maxDuration = 0.02f;

    [InspectorName("到时降级量")]
    [Tooltip("当buff持续时间到0时要降低的等级，如果设置为0则代表降至0级。")]
    public int demotion = 1;

    [InspectorName("自动刷新")]
    [Tooltip("当buff的等级上升时或达到最大等级后又提升等级时，是否自动刷新持续时间。")]
    public bool autoRefresh = true;

    [InspectorName("冲突处理")]
    [Tooltip("当两个不同来源者向同一个单位施加同一个buff时的冲突处理，详见BuffConflictResolution")]
    public BuffConflictResolution buffConflictResolution = BuffConflictResolution.Combine;


    [Header("用于分类处理")]
    [InspectorName("Buff种类：好坏中立")]
    [Tooltip("对buff进行分类处理：正面/负面/中性Buff")]
    public BuffType buffType = BuffType.Neutral;

    [InspectorName("Buff的稀有度")]
    [Tooltip("Buff的稀有度")]
    public BuffRarity buffRarity = BuffRarity.Common;

    [InspectorName("标签")]
    [Tooltip("此属性用于对buff进行额外需求Tag分类处理")]
    public string[] tags = null;

    [Header("用于UI显示")]
    [InspectorName("名称")]
    public string buffName = "未知的BUff";

    [InspectorName("Buff 描述")]
    public string description = "未知的描述";


    [InspectorName("Icon路径")]
    public string iconPath = "Icon\\";
    private void OnValidate()
    {
        if (maxLevel <= 0)
        {
            Debug.LogError("buff的最大等级不能小于或等于0");
            maxLevel = 1;
        }
        if (maxDuration <= 0)
        {
            Debug.LogError("buff的最大持续时间不能小于或等于0");
            maxDuration = 0.02f;
        }
        if (demotion < 0)
        {
            Debug.LogError("buff的到时降级量不能小于0");
            demotion = 0;
        }
    }
}