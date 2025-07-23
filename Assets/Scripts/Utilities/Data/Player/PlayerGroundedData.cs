using System;
using UnityEngine;

namespace PlayerSystem
{
    [Serializable]
    public class PlayerGroundedData
    {
        [field: SerializeField]
        [field: Range(0, 25f)]
        public float BaseSpeed { get; private set; } = 5.0f;

        [field: SerializeField]
        public AnimationCurve SlopeSpeedAngles { get; private set; }

        [field: SerializeField]
        public PlayerRotationData BaseRotationData { get; private set; }

        [field: SerializeField]
        public PlayerRunData RunData { get; private set; }

        [field: SerializeField]
        public PlayerDashData DashData { get; private set; }

        [field: SerializeField]
        public PlayerStopData StopData { get; private set; }
    }
}

