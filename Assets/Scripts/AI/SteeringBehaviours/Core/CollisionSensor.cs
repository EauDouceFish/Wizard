using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// RayCast/ColliderCast�汾Collision�Զ�̽�⣬�ɸ�Ϊ����Collider�汾
/// </summary>
public class CollisionSensor : MonoBehaviour
{
    /// <summary>
    /// avoid self detected - bias
    /// </summary>
    public float rayStartBias = 0.5f;
    public float rayLength = 15f;

    /// <summary>
    /// ÿ��30�ȷ���һ�����߼���ϰ�
    /// </summary>
    public int rayRoundAmount = 12;
    /// <summary>
    /// ���ϰ����Layer
    /// </summary>
    public LayerMask collisionLayers;

    //public float sphereRadius = 0.5f;
    //public bool useSphereCast = false;


    /// <summary>
    /// �ҵ�û���ϰ���ķ���
    /// </summary>
    /// <param name="desiredDirection"></param>
    /// <param name="outDirection"></param>
    /// <returns></returns>
    public bool GetCollisionFreeDirection(Vector3 desiredDirection, out Vector3 outDirection)
    {
        // ��ȡ����
        desiredDirection.Normalize();
        outDirection = desiredDirection;

        // �쳣����
        if (desiredDirection == Vector3.zero) return false;

        // �˴���ʼ����һ�������Ƽ��ı���Ŀ�귽���ڼ���֮����󷵻�
        Vector3 bestDirection = Vector3.zero;

        Vector3 bestDirectionLeft = GetBestDirectionHalf(-1, desiredDirection);
        Vector3 bestDirectionRight = GetBestDirectionHalf(1, desiredDirection);

        if (Vector3.Dot(transform.forward, bestDirectionLeft) > Vector3.Dot(transform.forward, bestDirectionRight))
        {
            bestDirection = bestDirectionLeft;
        }
        else
        {
            bestDirection = bestDirectionRight;
        }

        if (bestDirection != desiredDirection)
        {
            // ��Ҫ�ı䷽��
            outDirection = bestDirection;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// ���ߣ��Ұ�߼��һ�½Ƕȣ�֮��ȴ�С���ú�����������Ƕȴ�С
    /// �����signΪ�����Ҳ�汾signΪ��
    /// </summary>
    /// <param name="sign"></param>
    /// <param name="desiredDirection"></param>
    /// <returns></returns>
    public Vector3 GetBestDirectionHalf(int sign, Vector3 desiredDirection)
    {
        Vector3 result = Vector3.zero;

        // 180�ȷ�������
        for (int i = 0; i < rayRoundAmount / 2; i++)
        {
            float angle = sign * 360f / rayRoundAmount * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * desiredDirection;

            bool collision;

            RaycastHit hit;
            collision = Physics.Raycast(transform.position + rayStartBias * direction,
                direction, out hit, rayLength, collisionLayers);

            if (collision)
            {
                Debug.DrawRay(transform.position, direction * hit.distance, Color.red);
            }
            else
            {
                // ��С�Ŀ��нǶ�
                Debug.DrawRay(transform.position, direction * rayLength, Color.green);
                result = direction;
                break;
            }
            //if (useSphereCast)
            //{
            //    RaycastHit hit;
            //    collision = Physics.SphereCast(transform.position + rayStartBias * direction,
            //        sphereRadius, direction, out hit, rayLength, collisionLayers);
            //    if (collision)
            //    {
            //        Debug.DrawRay(transform.position, direction * hit.distance, Color.red);
            //    }
            //    else
            //    {
            //        Debug.DrawRay(transform.position, direction * rayLength, Color.green);
            //        result = direction;
            //        break;
            //    }
            //}
            //else
            //{
                   
            //}



        }
        return result;
    }
}
