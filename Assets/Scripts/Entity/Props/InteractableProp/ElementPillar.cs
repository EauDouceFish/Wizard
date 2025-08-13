using PlayerSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

/// <summary>
/// 元素石柱，交互以获取元素的力量
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
            // 默认元素为自然
            //element = MagicElement.Nature;

            // 获取领域元素类型、颜色
            if (hexRealm != null)
            {
                element = hexRealm.GetRealmBiome().MagicElement;
            }

            Color themeColor = MagicSpellSystem.GetElementColor(element, storage.GetColorMaps());

            // 激活特效并染色
            if (triggerVFX != null)
            {
                triggerVFX.SetActive(true);
                VFXColorHelper.ApplyColorToVFX(triggerVFX, themeColor);
            }
            this.GetSystem<AudioSystem>().PlaySoundEffect(storage.GetElementPillarSounds(), 2f);

            // 给予玩家元素力量、提升难度等级
            this.SendCommand(new UnlockMagicElementCommand(element));
            this.SendCommand(new IncreaseDifficultyCommand(1));
        }
    }
}