using PlayerSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 召唤一座离开游戏的结束交互物品
/// </summary>
public class SummonGameEnderCommand : AbstractCommand
{
    Storage storage;
    MapModel mapModel;
    PlayerModel playerModel;
    GameCoreModel gameCoreModel;
    protected override void OnExecute()
    {
        storage = this.GetUtility<Storage>();
        mapModel = this.GetModel<MapModel>();
        playerModel = this.GetModel<PlayerModel>();
        gameCoreModel = this.GetModel<GameCoreModel>();
        if (gameCoreModel.isGameEnded) return;
        gameCoreModel.isGameEnded = true;

        GameObject gameEnderPrefab = storage.GetGameEnderPrefab();
        HexCell endCell = FindEndCell();

        // 从地下升起
        Vector3 summonGroundPos = GOExtensions.GetGroundPosition(endCell.transform.position) - (Vector3.up * 11.0f);
        Quaternion rotation = CalculateRotationFacingPlayer(summonGroundPos);

        GameObject.Instantiate(gameEnderPrefab, summonGroundPos, rotation);
    }

    private Quaternion CalculateRotationFacingPlayer(Vector3 gameEnderPosition)
    {
        Player player = playerModel.GetPlayer();
        if (player == null)
        {
            Debug.LogWarning("未找到玩家!!");
            return Quaternion.identity;
        }

        Vector3 playerPosition = player.transform.position;
        Vector3 directionToPlayer = (playerPosition - gameEnderPosition).normalized;

        // 只在XZ平面,忽略Y轴
        directionToPlayer.y = 0;

        return Quaternion.LookRotation(directionToPlayer);
    }


    private HexCell FindEndCell()
    {
        foreach (var kvp in mapModel.HexGrid.allHexCellCoordDict)
        {
            HexCell cell = kvp.Value;
            if (cell.isEndLocation)
            {
                return cell;
            }
        }
        return null;
    }
}
