using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class Prop_HolyBlessingWater : InteractableEntity
{
    [SerializeField] GameObject thisObject;
    [SerializeField] GameObject blessingVFX;
    public override void OnInteract(GameObject player)
    {
        base.OnInteract(player);
        // ������������Buff 3ѡ1
        // �Ȳ���
        this.SendCommand(new GetHolyBlessingCommand());
        this.GetSystem<AudioSystem>().PlaySoundEffect(this.GetUtility<Storage>().GetBubbleSounds());
        Destroy(thisObject);
    }
}
