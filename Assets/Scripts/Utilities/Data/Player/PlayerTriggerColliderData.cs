using System;
using UnityEngine;

namespace PlayerSystem
{
    [Serializable]
    public class PlayerTriggerColliderData
    {
        /// <summary>
        /// 一个BoxCollider用于检测地面碰撞,这样可以避免卡入微小缝隙
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
