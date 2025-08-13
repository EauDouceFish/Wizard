using UnityEngine;

public class Prop_SlowDownBuff : InteractableEntity
{
    public override void OnInteract(GameObject interactor)
    {
        base.OnInteract(interactor);

        var buffable = interactor.GetComponent<IBuffableEntity>();
        if (buffable != null)
        {
            // 挂载Buff_SlowDown，provider可用道具名或自定义
            buffable.BuffSystem.AddBuff<SlowDown>("Prop_SlowDownBuff", 1);
        }
    }
}