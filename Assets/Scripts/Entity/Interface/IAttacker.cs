using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public interface IAttacker
{
    float CurrentAttack { get; set; }
    LayerMask AttackLayerMask { get; }
}