using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class MagicSpellModel : AbstractModel
{
    Storage storage;

    // 打击目标上的反应的增伤倍率
    public float FireWaterReactionMultiplier { get; set; } = 2.0f;
    public float FireIceReactionMultiplier { get; set; } = 1.8f;
    public float IceWaterReactionMultiplier { get; set; } = 1.0f;
    public float NatureRockReactionMultiplier { get; set; } = 1.5f;

    // 法术中反应的增伤倍率
    public float FireIceReactionMultiplierInSpell { get; set; } = 1.2f;
    public float FireWaterReactionMultiplierInSpell { get; set; } = 1.3f;
    public float IceWaterReactionMultiplierInSpell { get; set; } = 1.0f;

    // 基础法术中，每个元素增伤倍率(一条咒语最多五个元素)
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
