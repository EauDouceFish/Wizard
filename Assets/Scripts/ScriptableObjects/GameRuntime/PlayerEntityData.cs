using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

[CreateAssetMenu(fileName = "PlayerEntityData", menuName = "EntitySystem/PlayerEntityData")]
public class PlayerEntityData : ScriptableObject
{
    [Header("����ʵ������")]
    [Tooltip("ʵ������")]
    public string playerName = "Default Player";

    [Tooltip("��һ���Ѫ��")]
    public int playerBaseHealth = 50;
 
    [Tooltip("��һ���������")]
    public int playerBaseAttack = 10;
}
