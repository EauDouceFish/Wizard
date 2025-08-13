using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

public class CharacterGrowthSystem : AbstractSystem
{
    Storage storage;
    protected override void OnInit()
    {
        storage = this.GetUtility<Storage>();
        this.RegisterEvent<OnBattleRegionClearedEvent>(SummonRegionReward);
    }

    private void SummonRegionReward(OnBattleRegionClearedEvent evt)
    {
        HexCell targetCell = evt.hexCell;
        Vector3 summonPos = GOExtensions.GetGroundPosition(targetCell.transform.position);
        GameObject rewardPrefab = storage.GetHolyBlessingWaterPrefab();
        GameObject.Instantiate(rewardPrefab, summonPos, Quaternion.identity);
    }
}
