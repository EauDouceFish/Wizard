using System;
using UnityEngine;

namespace PlayerSystem
{
    [Serializable]
    public class PlayerRotationData
    {
        /// <summary>
        /// 平滑转向时长
        /// </summary>
        [field: SerializeField]
        public Vector3 TargetRotationReachTime { get; private set; }
    }
}
