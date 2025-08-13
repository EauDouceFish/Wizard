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
    /// ����cell��entityGroup���ٻ�һ��ʵ�壬���泯������򣬲��������������(+-)��������������򶼸��ݷ�Χ���ֱ���һ�����ƫ��
    /// </summary>
    public GameObject SummonGameObjectFacingHexCellCenter(HexCell cell, GameObject gameObject)
    {
        // ��������HexCell���ĵķ���
        Vector3 cellCenter = cell.transform.position;
        Vector3 currentPosition = transform.position;
        Vector3 directionToCenter = (cellCenter - currentPosition).normalized;

        // ����Ŀ�����ת
        Quaternion lookRotation = Quaternion.LookRotation(directionToCenter);

        // ����ƫ�ƣ���ֱ�ڳ�����
        Vector3 rightDirection = Vector3.Cross(directionToCenter, Vector3.up).normalized;
        float horizontalOffset = Random.Range(-maxOffsetDistance, maxOffsetDistance);
        Vector3 horizontalOffsetVector = rightDirection * horizontalOffset;

        // ����ƫ�ƣ��س�����
        float verticalOffset = Random.Range(minOffsetDistance, maxOffsetDistance);
        Vector3 verticalOffsetVector = directionToCenter * verticalOffset;

        // ��������λ��
        Vector3 finalPosition = currentPosition + horizontalOffsetVector + verticalOffsetVector;
        finalPosition = GOExtensions.GetGroundPosition(finalPosition) + Vector3.up;

        // ʵ����ʵ��Clone
        GameObject instance = Instantiate(gameObject, finalPosition, lookRotation);
        return instance;
    }
}
