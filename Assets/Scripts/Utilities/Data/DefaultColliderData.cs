using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    // ��Ҫ�߶ȣ����ĺͰ뾶���ɶ���Collider
    [Serializable]
    public class DefaultColliderData
    {
        [field: SerializeField] public float Height { get; private set; } = 1.0f;
        [field: SerializeField] public float CenterY { get; private set; } = 0.8f;
        [field: SerializeField] public float Radius { get; private set; } = 0.2f;
    }
}
