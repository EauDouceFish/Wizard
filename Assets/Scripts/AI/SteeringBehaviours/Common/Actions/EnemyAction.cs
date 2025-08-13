using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAction : Action
{
    public string AnimationParamName = "";

    protected Rigidbody body;
    protected Animator animator;
    protected Enemy enemy;

    public override void OnAwake()
    {
        base.OnAwake();
        body = GetComponent<Rigidbody>();
        enemy = GetComponent<Enemy>();
    }

    public override void OnStart()
    {
        base.OnStart();
        animator = enemy.GetAnimator();
    }

    protected void SetAnimationBool(bool value)
    {
        if (animator != null && !string.IsNullOrEmpty(AnimationParamName))
        {
            animator.SetBool(AnimationParamName, value);
        }
    }

    protected void SetAnimationTrigger()
    {
        if (animator != null && !string.IsNullOrEmpty(AnimationParamName))
        {
            animator.SetTrigger(AnimationParamName);
        }
    }
}