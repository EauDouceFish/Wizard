using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// 重定向最后一个领域的中心
/// </summary>
public class RelocateBossStep : IMapGenerationStep, ICanGetModel
{
    Storage storage;

    public void Execute(MapModel mapModel)
    {
        storage = mapModel.GetUtility<Storage>();
        List<HexRealm> hexRealms = mapModel.HexGrid.GetHexRealms();
        HexRealm lastRealm = hexRealms[hexRealms.Count - 1];
        HexCell realmEntrance = FindRealmEntrance(mapModel, lastRealm);
        lastRealm.ResetRealmCenter(realmEntrance);
        
        HexCell bossHexCell = FindFarthestCell(mapModel, lastRealm, realmEntrance);
        bossHexCell.isEndLocation = true;

        GameCoreModel gameCoreModel = this.GetModel<GameCoreModel>();
        gameCoreModel.EndHexCell = bossHexCell;
        // 生成Boss
        //GameObject gameObject = storage.GetBossEnemyPrefab();
        //Vector3 pos = GOExtensions.GetGroundPosition(bossHexCell.transform.position);
        //GameObject boss = GameObject.Instantiate(gameObject, bossHexCell.transform.position, Quaternion.identity);
    }

    // 利用之前记录的mainpath找到领域的入口
    private HexCell FindRealmEntrance(MapModel mapModel, HexRealm targetRealm)
    {
        List<HexCell> mainPath = mapModel.MainPath;

        foreach (HexCell cell in mainPath)
        {
            if (cell.HexRealm == targetRealm)
            {
                return cell;
            }
        }
        return null;
    }

    private HexCell FindFarthestCell(MapModel mapModel, HexRealm targetRealm, HexCell startCell)
    {
        float maxDistance = 0f;
        HexCell farthestCell = null;
        foreach (HexCell cell in targetRealm.GetHexCellsInRealm())
        {
            if (cell == startCell) continue;
            float distance = Vector3.Distance(startCell.transform.position, cell.transform.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestCell = cell;
            }
        }
        return farthestCell;
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
