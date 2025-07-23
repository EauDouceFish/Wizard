using Cinemachine;
using PlayerSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonPlayerCommand : AbstractCommand
{
    MapModel mapModel;
    Storage storage;
    GameObject player;
    HexGrid hexGrid;
    CinemachineVirtualCamera virtualCamera;
    protected override void OnExecute()
    {
        Init();
        var hexRealms = hexGrid.GetHexRealms();
        HexRealm firstHexRealm = hexRealms[0];
        // 把玩家放在初始群系中心点上方
        Vector3 spawnPosition = firstHexRealm.GetRealmCenterUponGround() + Vector3.up * 0.5f;

        GameObject existingPlayer = GameObject.FindWithTag("Player");
        if (existingPlayer != null)
        {
            existingPlayer.transform.position = spawnPosition;
            SetupVirtualCamera(existingPlayer.transform);
        }
        else
        {
            GameObject playerInstance = GameObject.Instantiate(player, spawnPosition, Quaternion.identity);
            SetupVirtualCamera(playerInstance.transform);
        }
    
    }

    private void Init()
    {
        mapModel = this.GetSystem<MapGenerationSystem>().GetMapModel();
        storage = this.GetUtility<Storage>();
        player = storage.GetPlayerPrefab();
        hexGrid = mapModel.HexGrid;
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void SetupVirtualCamera(Transform playerTransform)
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = playerTransform;
            virtualCamera.LookAt = playerTransform;
        }
    }
}
