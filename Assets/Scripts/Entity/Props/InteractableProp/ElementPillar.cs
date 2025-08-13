using PlayerSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

/// <summary>
/// Ԫ��ʯ���������Ի�ȡԪ�ص�����
/// </summary>
public class ElementPillar : InteractableEntity
{
    [SerializeField] GameObject triggerVFX;
    private HexRealm hexRealm;
    private Storage storage => this.GetUtility<Storage>();
    [Header("Debug")]
    public MagicElement element;

    public ElementPillar(HexRealm hexRealm)
    {
        this.hexRealm = hexRealm;
    }
    public override void OnInteract(GameObject interactor)
    {
        if (!canInteract) return;
        canInteract = false;

        Player player = interactor.GetComponent<Player>();
        if (player != null)
        {
            // Ĭ��Ԫ��Ϊ��Ȼ
            //element = MagicElement.Nature;

            // ��ȡ����Ԫ�����͡���ɫ
            if (hexRealm != null)
            {
                element = hexRealm.GetRealmBiome().MagicElement;
            }

            Color themeColor = MagicSpellSystem.GetElementColor(element, storage.GetColorMaps());

            // ������Ч��Ⱦɫ
            if (triggerVFX != null)
            {
                triggerVFX.SetActive(true);
                VFXColorHelper.ApplyColorToVFX(triggerVFX, themeColor);
            }
            this.GetSystem<AudioSystem>().PlaySoundEffect(storage.GetElementPillarSounds(), 2f);

            // �������Ԫ�������������Ѷȵȼ�
            this.SendCommand(new UnlockMagicElementCommand(element));
            this.SendCommand(new IncreaseDifficultyCommand(1));
        }
    }
}