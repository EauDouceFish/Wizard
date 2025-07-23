using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStoppingState : PlayerGroundedState
{
    public PlayerStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    #region IState Methods

    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = 0;

        // Camera

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.StoppingParameterHash);
    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.StoppingParameterHash);

    }

    // ����ϵͳ�ڶ�ˮƽ������м���
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // ���ø��෽��ƽ��ת��
        RotateTowardsTargetRotation();
        //Logger.Log(stateMachine.Player.Rigidbody.velocity.ToString());

        if (!IsMovingHorizontally())
        {
            return;
        }

        DecelerateHorizontally();
    }

    public override void OnAnimationTransitionEvent()
    {
        base.OnAnimationTransitionEvent();
        stateMachine.ChangeState(stateMachine.IdlingState);
    }

    #endregion

    #region Reusable Methods

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        //stateMachine.Player.Input.PlayerActions.Movement.started += OnMovementStarted;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        //stateMachine.Player.Input.PlayerActions.Movement.started -= OnMovementStarted;
    }

    #endregion

    #region Input Methods

    private void OnMovementStarted(InputAction.CallbackContext context)
    {
        OnMove();
    }


    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
    }

    #endregion
}
