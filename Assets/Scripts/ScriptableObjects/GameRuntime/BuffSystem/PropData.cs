using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

[CreateAssetMenu(fileName = "PropData", menuName = "BuffSystem/PropData")]
public class PropData : ScriptableObject
{
    public Sprite icon = null;
    public string propName = "";
    public string propDesc = "";
    public int rarity = 1;
    [Tooltip("一局游戏内最大出现次数")]
    public int maxAvailableCount = 100;
}
