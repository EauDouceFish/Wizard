using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ϊ ���ƽ���ƶ�
/// 1.��ɫ��Χ��һ��ԲȦ��Χ
/// 2.��ԲȦ������Ŀ��λ��
/// 3.ƽ���ƶ���ȥ
/// 4.�������ʱ���ظ��ù���
/// </summary>
public class WanderBehaviour : MonoBehaviour
{
    // 1.ԲȦ�İ뾶
    public float wanderRadius = 2;
    // 4.���ʱ��ķ�Χ
    public Vector2 targetChangeRange = new Vector2(2, 6);
    //public float targetHeight

    // Ŀ��λ��
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
    /// ���ʱ���ڶ��ڸ���Ŀ��λ��
    /// </summary>
    /// <returns></returns>
    IEnumerator TargetPositionChange()
    {
        while (true)
        {
            // ����ƽ�浥λԲ,���ѡ����,����Ŀ��λ��
            float theta = Random.value * 2 * Mathf.PI;
            Vector3 wanderTarget = new Vector3(wanderRadius * Mathf.Cos(theta), 0, wanderRadius * Mathf.Sin(theta));
            wanderTarget.Normalize();
            wanderTarget *= wanderRadius;
            targetPosition = transform.position + wanderTarget;

            // ���ʱ���ظ��������
            yield return new WaitForSeconds(Random.Range(targetChangeRange.x, targetChangeRange.y));
        }
    }

    /// <summary>
    /// ��ȡ������Ϊ�ļ��ٶ�
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSteering()
    {
        Debug.DrawLine(transform.position, targetPosition, Color.gray);
        return steeringBehaviours.Seek(targetPosition);
    }
}
