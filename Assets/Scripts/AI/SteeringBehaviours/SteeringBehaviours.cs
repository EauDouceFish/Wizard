using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

/// <summary>
/// AI的基础移动
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class SteeringBehaviours : MonoBehaviour
{
    // 基本属性：
    // 最大速度，最大加速度，转向速度
    [Header("General")]
    public float maxVelocity = 3.5f;
    public float maxAcceleration = 20f;
    public float turnSpeed = 20.0f;

    [Header("Arrive")]
    public float targetRadius = 0.005f;
    public float slowRadius = 1.0f;
    public float timeToTarget = 0.1f;

    private float speedMultiplier = 1.0f;

    private float originMaxVelocity;

    private Rigidbody rb;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originMaxVelocity = maxVelocity;
    }

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            // 尝试递归搜索子对象中的 SpriteRenderer
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError($"{name}: SpriteRenderer component is missing on this GameObject or its children!");
                return;
            }
        }
    }

    /// <summary>
    /// 传入不同的行为方法模拟得到最大加速度 进行线性加速 无法超过最大速度
    /// 记得关闭重力
    /// </summary>
    /// <param name="linearAcceleration"></param>
    public void Steer(Vector3 linearAcceleration)
    {
        // 应用加速度并调整速度
        rb.velocity += linearAcceleration * Time.deltaTime;

        // 将速度限制在修正后的最大速度范围内
        float adjustedMaxVelocity = maxVelocity * speedMultiplier;
        if (rb.velocity.magnitude > adjustedMaxVelocity)
        {
            rb.velocity = rb.velocity.normalized * adjustedMaxVelocity;
        }
    }


    /// <summary>
    /// 以给定加速度前往目的地
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="seekAccel"></param>
    /// <returns></returns>
    public Vector3 Seek(Vector3 targetPosition, float seekAccel)
    {
        Vector3 accelerationTarget = targetPosition - transform.position;
        accelerationTarget.Normalize();
        accelerationTarget *= seekAccel;
        return accelerationTarget;
    }

    /// <summary>
    /// 以最大加速度前往目的地
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public Vector3 Seek(Vector3 targetPosition)
    {
        return Seek(targetPosition, maxAcceleration);
    }


    /// <summary>
    /// 类似Seek，但靠近目标物体时候减速
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public Vector3 Arrive(Vector3 targetPosition)
    {
        Vector3 targetVelocity = targetPosition - rb.position;
        float dist = targetVelocity.magnitude;

        if (dist < targetRadius)
        {
            rb.velocity = Vector3.zero;
            return Vector3.zero;
        }

        float targetSpeed;
        if (dist > slowRadius)
        {
            targetSpeed = maxVelocity * speedMultiplier;
        }
        else
        {
            targetSpeed = (maxVelocity * speedMultiplier) * (dist / slowRadius);
        }

        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        Vector3 acceleration = targetVelocity - rb.velocity;
        acceleration *= 1 / timeToTarget;

        if (acceleration.magnitude > maxAcceleration)
        {
            acceleration = acceleration.normalized * maxAcceleration;
        }

        return acceleration;
    }

    /// <summary>
    /// 转向并移动到目标方向
    /// </summary>
    public void LookMoveDirection()
    {
        Vector3 direction = rb.velocity;

        LookAtDirection(direction);
    }

    /// <summary>
    /// Sprite转向并移动到目标方向
    /// </summary>
    public void LookMoveDirectionSprite()
    {
        Vector3 direction = rb.velocity;

        // 使用新的 LookAtDirectionSprite，只根据目标水平位置判断朝向
        LookAtDirectionSprite(direction);
    }

    /// <summary>
    /// 3D物体转向目标direction，直接修改rotation会强制转向（与刚体旋转禁止与否无关）
    /// </summary>
    /// <param name="direction"></param>
    public void LookAtDirection(Vector3 direction)
    {
        direction.Normalize();

        if (direction.sqrMagnitude > 0.001f)
        {
            float toRotation = -1 * (Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg) + 90;
            float rotation = Mathf.LerpAngle(transform.rotation.eulerAngles.y, toRotation, Time.deltaTime * turnSpeed);

            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }

    /// <summary>
    /// Sprite转向目标方向，仅在水平方向上翻转Sprite。
    /// </summary>
    /// <param name="targetPosition">目标位置</param>
    public void LookAtDirectionSprite(Vector3 targetPosition)
    {
        // 获取目标位置与当前物体位置的水平差距
        float directionByX = targetPosition.x;

        // 根据方向设置 flipX
        if (directionByX < 0)
        {
            spriteRenderer.flipX = true; // 面向左
        }
        else if (directionByX > 0)
        {
            spriteRenderer.flipX = false; // 面向右
        }
    }

    public Vector3 Interpose(Rigidbody target1, Rigidbody target2)
    {
        Vector3 midPoint = (target1.position + target2.position) / 2;

        float timeToReachMidPoint = Vector3.Distance(midPoint, transform.position) / maxVelocity;

        Vector3 futureTarget1Pos = target1.position + target1.velocity * timeToReachMidPoint;
        Vector3 futureTarget2Pos = target2.position + target2.velocity * timeToReachMidPoint;

        midPoint = (futureTarget1Pos + futureTarget2Pos) / 2;

        return Arrive(midPoint);
    }

    /// <summary>
    /// 减少multiplier倍的移动速度
    /// </summary>
    /// <param name="multiplierSubFactor"></param>
    public void SubSpeedMultiplier(float multiplierSubFactor)
    {
        speedMultiplier -= multiplierSubFactor;
        if (speedMultiplier < 0)
        {
            speedMultiplier = 0;
        }
        maxVelocity = originMaxVelocity;
        maxVelocity *= speedMultiplier;
        Debug.Log($"SpeedMultiplier: {speedMultiplier}, MaxVelocity: {maxVelocity}");
    }

    /// <summary>
    /// 增加multiplier倍的移动速度
    /// </summary>
    /// <param name="multiplierAddFactor"></param>
    public void AddSpeedMultiplier(float multiplierAddFactor)
    {
        speedMultiplier += multiplierAddFactor;
        maxVelocity = originMaxVelocity;
        maxVelocity *= speedMultiplier;
        Debug.Log($"SpeedMultiplier: {speedMultiplier}, MaxVelocity: {maxVelocity}");
    }

    /// <summary>
    /// 恢复正常速度
    /// </summary>
    public void ResetSpeedMultiplier()
    {
        speedMultiplier = 1.0f; // 恢复正常速度
        maxVelocity = originMaxVelocity;
        Debug.Log($"SpeedMultiplier: {speedMultiplier}, MaxVelocity: {maxVelocity}");
    }

}
