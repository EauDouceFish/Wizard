using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

[CreateAssetMenu(fileName = "PlayerEntityData", menuName = "EntitySystem/PlayerEntityData")]
public class PlayerEntityData : ScriptableObject
{
    [Header("基础实体属性")]
    [Tooltip("实体名称")]
    public string playerName = "Default Player";

    [Tooltip("玩家基础血量")]
    public int playerBaseHealth = 50;
 
    [Tooltip("玩家基础攻击力")]
    public int playerBaseAttack = 10;
}
