using System;
using UnityEngine;

public class BuffProvider : MonoBehaviour, IBuffProvider
{
    [SerializeField] private string providerName = "����ҩˮ";
    //[SerializeField] private int stackCount = 1;

    public string ProviderName => providerName;

    public void ApplyBuffToEntity(IBuffableEntity entity)
    {
        //entity.BuffSystem.AddBuff<>(ProviderName, stackCount);
    }
}