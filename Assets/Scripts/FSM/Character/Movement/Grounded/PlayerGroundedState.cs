using PlayerSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ��װһ��Grounded״̬��״̬�ڼ���ʹ����ͬshouldWalk�߼�
/// - Ĭ��ֹͣ�ƶ������ʱ���л�ΪIdle
/// </summary>
public class PlayerGroundedState : PlayerMovementState
{
    private SlopeData slopeData;

    public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        slopeData = stateMachine.Player.ColliderUtility.SlopeData;
    }

    #region IState Methods

    public override void Enter()
    {
        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.GroundedParameterHash);

        // �������
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.GroundedParameterHash);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        FloatCapsule();
    }

    #endregion

    #region Main Methods

    // ����Ҳ��ᱻС�¿�ס
    private void FloatCapsule()
    {
        Vector3 capsuleColliderCenterInWorldSpace =
            stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        // Ͷ������ͬʱ���Դ�����
        if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit,
            slopeData.FloatRayDistance, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            // �����¶�
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

            if (slopeSpeedModifier == 0f)
            {
                return;
            }

            float distanceToFloatPoint = stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderCenterInLocalSpace.y
                * stateMachine.Player.transform.localScale.y - hit.distance;

            if (distanceToFloatPoint == 0f)
            {
                return;
            }

            float amountToLift = distanceToFloatPoint * slopeData.StepReachForce - GetPlayerVerticalVelocity().y;

            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);

            stateMachine.AddForce(liftForce, ForceMode.VelocityChange);
        }
    }

    // ʹ��AnimationCurveʵ�ֹ��ɱ仯���ٶȿ���
    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        float slopeSpeedModifier = movementData.SlopeSpeedAngles.Evaluate(angle);

        stateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;

        return slopeSpeedModifier;
    }

    #endregion

    #region Reusable Methods

    /// <summary>
    /// ����״̬��¼�ɴ����Ϊ���ƶ�����̡���Ծ ��ת��
    /// </summary>
    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        // �ƶ��ˣ���¼�ƶ�״̬�л�
        // stateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCanceled;

        // ��¼���״̬��ʼ
        stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        // stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCanceled;
        stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;
    }


    /// <summary>
    /// �ƶ���״̬��������ʱ��ͨ��shouldWalk�ж�״̬�л���������/�ܲ�
    /// </summary>
    protected virtual void OnMove()
    {
        // ʹ��Data����
        //if (stateMachine.ReusableData.ShouldWalk)
        //{
        //    stateMachine.ChangeState(stateMachine.WalkingState);

        //    return;
        //}
        //stateMachine.ChangeState(stateMachine.RunningState);
    }

    /// <summary>
    /// ���������ת��Ϣ
    /// </summary>
    protected void SetBaseRotationData()
    {
        stateMachine.ReusableData.RotationData = movementData.BaseRotationData;

        stateMachine.ReusableData.TimeToReachTargetRotation =
            stateMachine.ReusableData.RotationData.TargetRotationReachTime;
    }


    #endregion

    #region Input Methods

    // �˳��ƶ�״̬ ���ؾ�ֹ
    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.IdlingState);
    }

    protected virtual void OnDashStarted(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.DashingState);
    }

    #endregion
}
