using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 漫游行为 随机平滑移动
/// 1.角色周围有一个圆圈范围
/// 2.在圆圈上生成目标位置
/// 3.平滑移动过去
/// 4.不断随机时间重复该过程
/// </summary>
public class WanderBehaviour : MonoBehaviour
{
    // 1.圆圈的半径
    public float wanderRadius = 2;
    // 4.随机时间的范围
    public Vector2 targetChangeRange = new Vector2(2, 6);
    //public float targetHeight

    // 目标位置
    Vector3 targetPosition;

    SteeringBehaviours steeringBehaviours;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        steeringBehaviours = GetComponent<SteeringBehaviours>();
    }
    private void Start()
    {
        StartCoroutine(TargetPositionChange());
    }

    /// <summary>
    /// 随机时间内定期更新目标位置
    /// </summary>
    /// <returns></returns>
    IEnumerator TargetPositionChange()
    {
        while (true)
        {
            // 构建平面单位圆,随机选择方向,设置目标位置
            float theta = Random.value * 2 * Mathf.PI;
            Vector3 wanderTarget = new Vector3(wanderRadius * Mathf.Cos(theta), 0, wanderRadius * Mathf.Sin(theta));
            wanderTarget.Normalize();
            wanderTarget *= wanderRadius;
            targetPosition = transform.position + wanderTarget;

            // 随机时间重复这个过程
            yield return new WaitForSeconds(Random.Range(targetChangeRange.x, targetChangeRange.y));
        }
    }

    /// <summary>
    /// 获取漫游行为的加速度
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSteering()
    {
        Debug.DrawLine(transform.position, targetPosition, Color.gray);
        return steeringBehaviours.Seek(targetPosition);
    }
}
