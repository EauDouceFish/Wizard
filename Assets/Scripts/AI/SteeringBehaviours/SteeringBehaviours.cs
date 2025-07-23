using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

/// <summary>
/// AI�Ļ����ƶ�
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class SteeringBehaviours : MonoBehaviour
{
    // �������ԣ�
    // ����ٶȣ������ٶȣ�ת���ٶ�
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
            // ���Եݹ������Ӷ����е� SpriteRenderer
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError($"{name}: SpriteRenderer component is missing on this GameObject or its children!");
                return;
            }
        }
    }

    /// <summary>
    /// ���벻ͬ����Ϊ����ģ��õ������ٶ� �������Լ��� �޷���������ٶ�
    /// �ǵùر�����
    /// </summary>
    /// <param name="linearAcceleration"></param>
    public void Steer(Vector3 linearAcceleration)
    {
        // Ӧ�ü��ٶȲ������ٶ�
        rb.velocity += linearAcceleration * Time.deltaTime;

        // ���ٶ������������������ٶȷ�Χ��
        float adjustedMaxVelocity = maxVelocity * speedMultiplier;
        if (rb.velocity.magnitude > adjustedMaxVelocity)
        {
            rb.velocity = rb.velocity.normalized * adjustedMaxVelocity;
        }
    }


    /// <summary>
    /// �Ը������ٶ�ǰ��Ŀ�ĵ�
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
    /// �������ٶ�ǰ��Ŀ�ĵ�
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public Vector3 Seek(Vector3 targetPosition)
    {
        return Seek(targetPosition, maxAcceleration);
    }


    /// <summary>
    /// ����Seek��������Ŀ������ʱ�����
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
    /// ת���ƶ���Ŀ�귽��
    /// </summary>
    public void LookMoveDirection()
    {
        Vector3 direction = rb.velocity;

        LookAtDirection(direction);
    }

    /// <summary>
    /// Spriteת���ƶ���Ŀ�귽��
    /// </summary>
    public void LookMoveDirectionSprite()
    {
        Vector3 direction = rb.velocity;

        // ʹ���µ� LookAtDirectionSprite��ֻ����Ŀ��ˮƽλ���жϳ���
        LookAtDirectionSprite(direction);
    }

    /// <summary>
    /// 3D����ת��Ŀ��direction��ֱ���޸�rotation��ǿ��ת���������ת��ֹ����޹أ�
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
    /// Spriteת��Ŀ�귽�򣬽���ˮƽ�����Ϸ�תSprite��
    /// </summary>
    /// <param name="targetPosition">Ŀ��λ��</param>
    public void LookAtDirectionSprite(Vector3 targetPosition)
    {
        // ��ȡĿ��λ���뵱ǰ����λ�õ�ˮƽ���
        float directionByX = targetPosition.x;

        // ���ݷ������� flipX
        if (directionByX < 0)
        {
            spriteRenderer.flipX = true; // ������
        }
        else if (directionByX > 0)
        {
            spriteRenderer.flipX = false; // ������
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
    /// ����multiplier�����ƶ��ٶ�
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
    /// ����multiplier�����ƶ��ٶ�
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
    /// �ָ������ٶ�
    /// </summary>
    public void ResetSpeedMultiplier()
    {
        speedMultiplier = 1.0f; // �ָ������ٶ�
        maxVelocity = originMaxVelocity;
        Debug.Log($"SpeedMultiplier: {speedMultiplier}, MaxVelocity: {maxVelocity}");
    }

}
