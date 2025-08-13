using System;
using UnityEngine;

public class Buff_AddBaseAttack_Provider : InteractableEntity, IBuffProvider
{
    [SerializeField] private string providerName = "����ף��";

    public string ProviderName => providerName;

    public void ApplyBuffToEntity(IBuffableEntity entity)
    {
        entity.BuffSystem.AddBuff<AddBaseAttack>(providerName);
    }

    public override void OnInteract(GameObject player)
    {
        base.OnInteract(player);
        IBuffableEntity entity = player.GetComponent<IBuffableEntity>();
        ApplyBuffToEntity(entity);
    }
}