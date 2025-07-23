using PlayerSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : PlayerMovingState
{
    public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    #region IState Methods

    // running״̬�»ָ�Ĭ���ƶ��ٶ�1.0f �������ƶ�״̬�µ�0.225f
    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = movementData.RunData.SpeedModifier;
        base.Enter();
        StartAnimation(stateMachine.Player.AnimationData.RunParameterHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.RunParameterHash);

    }

    public override void Update()
    {
        base.Update();
        if (stateMachine.ReusableData.HasClickTarget)
        {
            if (stateMachine.HasReachedClickTarget())
            {
                stateMachine.ClearClickTarget();
                Debug.Log("������Ŀ���");
                stateMachine.ChangeState(stateMachine.IdlingState);
                return;
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    #endregion

    #region Main Methods

    #endregion

    #region Input Methods


    //protected override void OnMovementCanceled(InputAction.CallbackContext context)
    //{
    //    stateMachine.ClearClickTarget();
    //    //stateMachine.ChangeState(stateMachine.MediumStoppingState);
    //}

    // �л���Walk״̬
    //protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    //{
    //    base.OnWalkToggleStarted(context);

    //    stateMachine.ChangeState(stateMachine.WalkingState);
    //}


    #endregion
}