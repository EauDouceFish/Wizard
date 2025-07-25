using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    [Serializable]
    public class SlopeData
    {
        [field: SerializeField]
        [field: Range(0f, 1f)]
        public float StepHeightPercentage { get; private set; } = 0.25f;

        // �������ľ�����
        [field: SerializeField]
        [field: Range(0f, 5f)]
        public float FloatRayDistance { get; private set; } = 2f;

        [field: SerializeField]
        [field: Range(0f, 50f)]
        public float StepReachForce { get; private set; } = 25f;
    }
}


