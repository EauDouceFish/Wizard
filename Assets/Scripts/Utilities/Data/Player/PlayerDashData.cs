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
        /// ������̼����ֵ
        /// </summary>
        [field: SerializeField]
        [field: Range(0f, 2f)]
        public float TimeToBeConsideredConsecutive { get; private set; } = 0.5f;

        /// <summary>
        /// ����֧��������̵Ĵ���
        /// </summary>
        [field: SerializeField]
        [field: Range(1f, 10f)]
        public float ConsecutiveDashesLimitAmount { get; private set; } = 1f;

        /// <summary>
        /// ������̺��ֹʹ�ó�̵�ʱ��
        /// </summary>
        [field: Range(1f, 3f)]
        public float DashLimitReachedCooldown { get; private set; } = 1.5f;
    }

}