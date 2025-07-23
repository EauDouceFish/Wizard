using System;
using UnityEngine;

namespace PlayerSystem
{
    [Serializable]
    public class PlayerCapsuleColliderUtility : CapsuleColliderUtility
    {
        // Player现在不做起跳
        [field: SerializeField] public PlayerTriggerColliderData TriggerColliderData { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            TriggerColliderData.Initialize();
        }
    }
}
