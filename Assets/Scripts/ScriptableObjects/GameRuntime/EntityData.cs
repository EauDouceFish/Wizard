using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "EntitySystem/EntityData")]
public class EntityData : ScriptableObject
{
    [Header("����ʵ������")]
    [Tooltip("ʵ������")]
    public string entityName = "Default Entity";

    [Tooltip("����Ѫ��")]
    public int baseHealth = 50;

    [Tooltip("ʵ��ͼ��")]
    public Sprite entityIcon;

    [Tooltip("ʵ��Ԥ����")]
    public GameObject entityPrefab;
}