using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// RayCast/ColliderCast版本Collision自动探测，可改为发射Collider版本
/// </summary>
public class CollisionSensor : MonoBehaviour
{
    /// <summary>
    /// avoid self detected - bias
    /// </summary>
    public float rayStartBias = 0.5f;
    public float rayLength = 15f;

    /// <summary>
    /// 每隔30度发射一条射线检测障碍
    /// </summary>
    public int rayRoundAmount = 12;
    /// <summary>
    /// 是障碍物的Layer
    /// </summary>
    public LayerMask collisionLayers;

    //public float sphereRadius = 0.5f;
    //public bool useSphereCast = false;


    /// <summary>
    /// 找到没有障碍物的方向
    /// </summary>
    /// <param name="desiredDirection"></param>
    /// <param name="outDirection"></param>
    /// <returns></returns>
    public bool GetCollisionFreeDirection(Vector3 desiredDirection, out Vector3 outDirection)
    {
        // 获取方向
        desiredDirection.Normalize();
        outDirection = desiredDirection;

        // 异常处理
        if (desiredDirection == Vector3.zero) return false;

        // 此处开始计算一个最终推荐的避障目标方向，在计算之后最后返回
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
            // 需要改变方向
            outDirection = bestDirection;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 左半边，右半边检测一下角度，之后比大小。该函数返回最近角度大小
    /// 左侧半边sign为负，右侧版本sign为正
    /// </summary>
    /// <param name="sign"></param>
    /// <param name="desiredDirection"></param>
    /// <returns></returns>
    public Vector3 GetBestDirectionHalf(int sign, Vector3 desiredDirection)
    {
        Vector3 result = Vector3.zero;

        // 180度发射射线
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
                // 最小的可行角度
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
