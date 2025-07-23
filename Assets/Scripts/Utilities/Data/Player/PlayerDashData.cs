using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    [Serializable]
    public class PlayerDashData
    {
        [field: SerializeField]
        [field: Range(1f, 3f)]
        public float SpeedModifier { get; private set; } = 2f;


        [field: SerializeField]
        [field: Range(0f, 1.5f)]
        public float Duration { get; private set; } = 0.1f;


        [field: SerializeField]
        public PlayerRotationData RotationData { get; private set; }

        /// <summary>
        /// 连续冲刺间隔阈值
        /// </summary>
        [field: SerializeField]
        [field: Range(0f, 2f)]
        public float TimeToBeConsideredConsecutive { get; private set; } = 0.5f;

        /// <summary>
        /// 最多可支持连续冲刺的次数
        /// </summary>
        [field: SerializeField]
        [field: Range(1f, 10f)]
        public float ConsecutiveDashesLimitAmount { get; private set; } = 1f;

        /// <summary>
        /// 连续冲刺后禁止使用冲刺的时间
        /// </summary>
        [field: Range(1f, 3f)]
        public float DashLimitReachedCooldown { get; private set; } = 1.5f;
    }

}