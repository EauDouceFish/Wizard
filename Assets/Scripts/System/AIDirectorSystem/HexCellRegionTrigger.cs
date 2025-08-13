using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexCellRegionTrigger : MonoBehaviour, IController, ICanSendEvent
{
    [SerializeField] HexCell hexCell;
    Storage storage;
    AudioSystem audioSystem;

    private void Awake()
    {
        storage = this.GetUtility<Storage>();
        audioSystem = this.GetSystem<AudioSystem>();
    }

    // ����ʱ�򣬱�Ǵ���
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Command�ɷ�װ����
            Debug.Log($"��ҽ�������{hexCell.coord.x}, {hexCell.coord.y})");
            this.SendEvent(new OnRegionEnterTriggerEvent { hexCell = this.hexCell });
            PlayerModel playerModel = this.GetModel<PlayerModel>();

            BiomeType realmBiomeType = hexCell.HexRealm.GetRealmBiome().BiomeType;

            if (playerModel.currentHexCell == null)
            {
                playerModel.currentHexCell = hexCell;
                audioSystem.FadeOutAndChangeMusic(GetAudioClipByRealmBiomeType(realmBiomeType), 1.0f);
            }
            else
            {
                // �������µ�HexRealm�����и�
                if (playerModel.currentHexCell.HexRealm != hexCell.HexRealm)
                {
                    audioSystem.FadeOutAndChangeMusic(GetAudioClipByRealmBiomeType(realmBiomeType), 2.0f);
                }
            }
            // EndCommand
        }
    }

    private AudioClip GetAudioClipByRealmBiomeType(BiomeType biomeType)
    {
        AudioClip newMusic = biomeType switch
        {
            BiomeType.Forest => storage.GetMusicNatureRegion(),
            BiomeType.Desert => storage.GetMusicDesertRegion(),
            BiomeType.Frost => storage.GetMusicFrostRegion(),
            BiomeType.Barren => storage.GetMusicBarrenRegion(),
            BiomeType.Volcano => storage.GetMusicVolcanoRegion(),
            _ => null
        };
        return newMusic;    
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
