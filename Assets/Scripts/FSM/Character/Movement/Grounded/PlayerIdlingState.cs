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

    // ��ֹ״̬����޷��ƶ�
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

        // ���ƶ����룬�����ƶ�״̬
        OnMove();
    }

    // �������������ٶ�
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // ��ֹ״̬������
        if (!IsMovingHorizontally())
        {
            return;
        }
        // Idle�»����ٶȣ�����������ٶ�
        ResetVelocity();
    }

    #endregion
}
