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
        Debug.Log("SummonPlayerCommand: 开始执行玩家生成命令");
        Init();
        var hexRealms = hexGrid.GetHexRealms();
        HexRealm firstHexRealm = hexRealms[0];
        // 把玩家放在初始群系中心点上方
        Vector3 spawnPosition = firstHexRealm.GetRealmCenterUponGround() + Vector3.up * 2f;

        Player playerInstance = null;
        GameObject existingPlayer = GameObject.FindWithTag("Player");
        if (existingPlayer != null)
        {
            existingPlayer.transform.position = spawnPosition;
            playerInstance = existingPlayer.GetComponent<Player>();
            SetupVirtualCamera(existingPlayer.transform);

            // 再移动一下玩家，每次都靠近相机而不是在正中心
            Vector3 direction = virtualCamera.transform.position - existingPlayer.transform.position;
            direction.y = 0;
            Vector3 newPlayerPos = existingPlayer.transform.position + direction.normalized * 15f;
            existingPlayer.transform.position = GOExtensions.GetGroundPosition(newPlayerPos) + Vector3.up * 2f;
        }
        else
        {
            Debug.LogError("场景内需要有Player用来摆放");
        }

        // 发送包含Player实例的事件
        this.SendEvent<PlayerCreatedEvent>(new PlayerCreatedEvent { player = playerInstance });

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
public struct PlayerCreatedEvent
{
    public Player player;
}