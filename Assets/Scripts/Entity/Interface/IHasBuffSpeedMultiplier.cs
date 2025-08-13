using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// 直接由Buff增加移动速度
/// </summary>
public interface IHasBuffSpeedMultiplier
{
    public float BuffSpeedMultiplier { get; set; }
}
