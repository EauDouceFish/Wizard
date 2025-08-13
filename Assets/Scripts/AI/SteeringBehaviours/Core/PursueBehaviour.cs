using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ץ����Ϊ����Ԥ���׷��Ŀ��
/// </summary>
public class PursueBehaviour : MonoBehaviour
{
    [SerializeField]
    private float maxPrediction = 1.0f;


    Rigidbody rb;
    SteeringBehaviours steeringBehaviours;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        steeringBehaviours = GetComponent<SteeringBehaviours>();
    }

    /// <summary>
    /// ����Ŀ������׷��,���������Ԥ��������׷��Ч��    
    /// /// </summary>
    public Vector3 GetSteering(Rigidbody target)
    {
        Vector3 displacement = target.position - transform.position;
        float distance = displacement.magnitude;
        float speed = rb.velocity.magnitude;

        float prediction = 0.0f;
        if (speed <= distance / maxPrediction)
        {
            //����̫Զ�Ͳ��Թ�Զ������Ԥ��
            prediction = maxPrediction;
        }
        else
        {
            prediction = distance / speed;

        }

        Vector3 explicitTarget = target.position + target.velocity * prediction;

        return steeringBehaviours.Seek(explicitTarget);
    }
}
