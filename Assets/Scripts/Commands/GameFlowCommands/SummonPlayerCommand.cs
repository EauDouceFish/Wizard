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
        Debug.Log("SummonPlayerCommand: ��ʼִ�������������");
        Init();
        var hexRealms = hexGrid.GetHexRealms();
        HexRealm firstHexRealm = hexRealms[0];
        // ����ҷ��ڳ�ʼȺϵ���ĵ��Ϸ�
        Vector3 spawnPosition = firstHexRealm.GetRealmCenterUponGround() + Vector3.up * 2f;

        Player playerInstance = null;
        GameObject existingPlayer = GameObject.FindWithTag("Player");
        if (existingPlayer != null)
        {
            existingPlayer.transform.position = spawnPosition;
            playerInstance = existingPlayer.GetComponent<Player>();
            SetupVirtualCamera(existingPlayer.transform);

            // ���ƶ�һ����ң�ÿ�ζ����������������������
            Vector3 direction = virtualCamera.transform.position - existingPlayer.transform.position;
            direction.y = 0;
            Vector3 newPlayerPos = existingPlayer.transform.position + direction.normalized * 15f;
            existingPlayer.transform.position = GOExtensions.GetGroundPosition(newPlayerPos) + Vector3.up * 2f;
        }
        else
        {
            Debug.LogError("��������Ҫ��Player�����ڷ�");
        }

        // ���Ͱ���Playerʵ�����¼�
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