using UnityEngine;

public class Prop_SlowDownBuff : InteractableEntity
{
    public override void OnInteract(GameObject interactor)
    {
        base.OnInteract(interactor);

        var buffable = interactor.GetComponent<IBuffableEntity>();
        if (buffable != null)
        {
            // ����Buff_SlowDown��provider���õ��������Զ���
            buffable.BuffSystem.AddBuff<SlowDown>("Prop_SlowDownBuff", 1);
        }
    }
}