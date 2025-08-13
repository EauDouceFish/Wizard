using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class EntityGroup : MonoBehaviour
{
    [SerializeField] float minOffsetDistance = 5.0f;

    [SerializeField] float maxOffsetDistance = 10.0f;

    GameObject specialObject;

    private readonly List<GameObject> entityObjects = new();

    public void AssignSpecialObject(GameObject specialObject)
    {
        this.specialObject = specialObject;
    }

    public void AddGameObjectEntity(GameObject entity)
    {
        entityObjects.Add(entity);
    }

    public void SummonSpecialObject()
    {
        if (specialObject != null)
        {
            specialObject.transform.position = GOExtensions.GetGroundPosition(transform.position);
            Instantiate(specialObject, transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// 连接cell和entityGroup，召唤一个实体，其面朝这个方向，并且在其自身横向(+-)、向该中心纵向方向都根据范围，分别作一个随机偏移
    /// </summary>
    public GameObject SummonGameObjectFacingHexCellCenter(HexCell cell, GameObject gameObject)
    {
        // 计算面向HexCell中心的方向
        Vector3 cellCenter = cell.transform.position;
        Vector3 currentPosition = transform.position;
        Vector3 directionToCenter = (cellCenter - currentPosition).normalized;

        // 面向目标的旋转
        Quaternion lookRotation = Quaternion.LookRotation(directionToCenter);

        // 横向偏移（垂直于朝向方向）
        Vector3 rightDirection = Vector3.Cross(directionToCenter, Vector3.up).normalized;
        float horizontalOffset = Random.Range(-maxOffsetDistance, maxOffsetDistance);
        Vector3 horizontalOffsetVector = rightDirection * horizontalOffset;

        // 纵向偏移（沿朝向方向）
        float verticalOffset = Random.Range(minOffsetDistance, maxOffsetDistance);
        Vector3 verticalOffsetVector = directionToCenter * verticalOffset;

        // 计算最终位置
        Vector3 finalPosition = currentPosition + horizontalOffsetVector + verticalOffsetVector;
        finalPosition = GOExtensions.GetGroundPosition(finalPosition) + Vector3.up;

        // 实例化实体Clone
        GameObject instance = Instantiate(gameObject, finalPosition, lookRotation);
        return instance;
    }
}
