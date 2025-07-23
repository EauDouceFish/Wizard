using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdlingState : PlayerGroundedState
{
    public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    #region IState Methods

    // 静止状态玩家无法移动
    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = 0.0f;

        // Camera

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.IdleParameterHash);

        ResetVelocity();
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.IdleParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            return;
        }

        // 有移动输入，进入移动状态
        OnMove();
    }

    // 物理修正残余速度
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // 静止状态则跳过
        if (!IsMovingHorizontally())
        {
            return;
        }
        // Idle下还有速度，则清除残余速度
        ResetVelocity();
    }

    #endregion
}
