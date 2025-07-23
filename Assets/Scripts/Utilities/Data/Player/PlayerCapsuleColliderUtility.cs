using System;
using UnityEngine;

namespace PlayerSystem
{
    [Serializable]
    public class PlayerCapsuleColliderUtility : CapsuleColliderUtility
    {
        // Player���ڲ�������
        [field: SerializeField] public PlayerTriggerColliderData TriggerColliderData { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            TriggerColliderData.Initialize();
        }
    }
}
