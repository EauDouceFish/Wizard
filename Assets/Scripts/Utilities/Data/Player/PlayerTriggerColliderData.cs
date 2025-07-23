using System;
using UnityEngine;

namespace PlayerSystem
{
    [Serializable]
    public class PlayerTriggerColliderData
    {
        /// <summary>
        /// һ��BoxCollider���ڼ�������ײ,�������Ա��⿨��΢С��϶
        /// </summary>
        [field: SerializeField]
        public BoxCollider GroundCheckCollider { get; private set; }

        public Vector3 GroundCheckColliderExtents { get; private set; }

        public void Initialize()
        {
            GroundCheckColliderExtents = GroundCheckCollider.bounds.extents;
        }
    }
}
