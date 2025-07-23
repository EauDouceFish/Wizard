using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    [Serializable]
    public class PlayerLayerData
    {
        [field: SerializeField] public LayerMask GroundLayer { get; private set; }

        // 1 ����layerλ����layerMask������&�Ƚϣ�ͬλΪ1�ŷ���1�����򷵻�0����������Layer
        public bool ContainsLayer(LayerMask layerMask, int layer)
        {
            return (1 << layer & layerMask) != 0;
        }

        public bool IsGroundLayer(int layer)
        {
            bool isGroundLayer = ContainsLayer(GroundLayer, layer);
            Debug.Log("IsGroundLayer: " + isGroundLayer);
            return isGroundLayer;
        }

    }

}
