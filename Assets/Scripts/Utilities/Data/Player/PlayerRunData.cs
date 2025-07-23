using System;
using UnityEngine;

namespace PlayerSystem
{
    [Serializable]
    public class PlayerRunData
    {
        [field: SerializeField]
        [field: Range(0f, 5f)]
        public float SpeedModifier { get; private set; } = 1.0f;
    }
}
