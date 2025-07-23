using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "EntitySystem/EntityData")]
public class EntityData : ScriptableObject
{
    [Header("基础实体属性")]
    [Tooltip("实体名称")]
    public string entityName = "Default Entity";

    [Tooltip("基础血量")]
    public int baseHealth = 50;

    [Tooltip("实体图标")]
    public Sprite entityIcon;

    [Tooltip("实体预制体")]
    public GameObject entityPrefab;
}