using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// 在指定 HexCell 召唤随机敌人的命令，敌人会被收纳到Model并且监听
/// </summary>
public class SummonRandomEnemiesAtHexCellCommand : AbstractCommand
{
    private HexCell targetHexCell;
    private int enemyCount;

    public SummonRandomEnemiesAtHexCellCommand(HexCell hexCell, int count)
    {
        targetHexCell = hexCell;
        enemyCount = count;
    }

    protected override void OnExecute()
    {
        var aiDirectorSystem = this.GetSystem<AIDirectorSystem>();
        aiDirectorSystem.SummonEnemiesAtHexCell(targetHexCell, enemyCount);
    }
}