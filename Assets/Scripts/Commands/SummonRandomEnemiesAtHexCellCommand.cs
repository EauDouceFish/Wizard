using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// ��ָ�� HexCell �ٻ�������˵�������˻ᱻ���ɵ�Model���Ҽ���
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