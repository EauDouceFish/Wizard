using System;
using UnityEngine;

namespace PlayerSystem
{
    [Serializable]
    public class PlayerRotationData
    {
        /// <summary>
        /// ƽ��ת��ʱ��
        /// </summary>
        [field: SerializeField]
        public Vector3 TargetRotationReachTime { get; private set; }
    }
}
