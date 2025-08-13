using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class MagicSpellModel : AbstractModel
{
    Storage storage;

    // ���Ŀ���ϵķ�Ӧ�����˱���
    public float FireWaterReactionMultiplier { get; set; } = 2.0f;
    public float FireIceReactionMultiplier { get; set; } = 1.8f;
    public float IceWaterReactionMultiplier { get; set; } = 1.0f;
    public float NatureRockReactionMultiplier { get; set; } = 1.5f;

    // �����з�Ӧ�����˱���
    public float FireIceReactionMultiplierInSpell { get; set; } = 1.2f;
    public float FireWaterReactionMultiplierInSpell { get; set; } = 1.3f;
    public float IceWaterReactionMultiplierInSpell { get; set; } = 1.0f;

    // ���������У�ÿ��Ԫ�����˱���(һ������������Ԫ��)
    public float FireElementMultiplier { get; set; } = 0.15f;
    public float WaterElementMultiplier { get; set; } = 0.1f;
    public float IceElementMultiplier { get; set; } = 0.1f;
    public float NatureElementMultiplier { get; set; } = 0.15f;
    public float RockElementMultiplier { get; set; } = 0.1f;

    protected override void OnInit()
    {
        storage = this.GetUtility<Storage>();
    }
}
