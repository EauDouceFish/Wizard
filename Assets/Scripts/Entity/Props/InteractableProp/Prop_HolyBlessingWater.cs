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
        // 给玩家随机永久Buff 3选1
        // 先测试
        this.SendCommand(new GetHolyBlessingCommand());
        this.GetSystem<AudioSystem>().PlaySoundEffect(this.GetUtility<Storage>().GetBubbleSounds());
        Destroy(thisObject);
    }
}
