using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抓捕行为，带预测的追上目标
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
    /// 传入目标向其追逐,可配置最大预测距离调整追逐效果    
    /// /// </summary>
    public Vector3 GetSteering(Rigidbody target)
    {
        Vector3 displacement = target.position - transform.position;
        float distance = displacement.magnitude;
        float speed = rb.velocity.magnitude;

        float prediction = 0.0f;
        if (speed <= distance / maxPrediction)
        {
            //距离太远就不对过远处做出预测
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
