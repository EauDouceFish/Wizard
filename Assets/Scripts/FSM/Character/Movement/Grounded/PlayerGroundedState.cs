using PlayerSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 封装一个Grounded状态，状态内即可使用相同shouldWalk逻辑
/// - 默认停止移动输入的时候切换为Idle
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

        // 操作相机
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

    // 让玩家不会被小坡卡住
    private void FloatCapsule()
    {
        Vector3 capsuleColliderCenterInWorldSpace =
            stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        // 投射射线同时忽略触发器
        if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit,
            slopeData.FloatRayDistance, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            // 计算坡度
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

    // 使用AnimationCurve实现规律变化的速度控制
    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        float slopeSpeedModifier = movementData.SlopeSpeedAngles.Evaluate(angle);

        stateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;

        return slopeSpeedModifier;
    }

    #endregion

    #region Reusable Methods

    /// <summary>
    /// 地面状态记录可打断行为：移动、冲刺、跳跃 的转换
    /// </summary>
    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        // 移动端：记录移动状态切换
        // stateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCanceled;

        // 记录冲刺状态开始
        stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        // stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCanceled;
        stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;
    }


    /// <summary>
    /// 移动端状态接受输入时：通过shouldWalk判断状态切换到：行走/跑步
    /// </summary>
    protected virtual void OnMove()
    {
        // 使用Data数据
        //if (stateMachine.ReusableData.ShouldWalk)
        //{
        //    stateMachine.ChangeState(stateMachine.WalkingState);

        //    return;
        //}
        //stateMachine.ChangeState(stateMachine.RunningState);
    }

    /// <summary>
    /// 设置玩家旋转信息
    /// </summary>
    protected void SetBaseRotationData()
    {
        stateMachine.ReusableData.RotationData = movementData.BaseRotationData;

        stateMachine.ReusableData.TimeToReachTargetRotation =
            stateMachine.ReusableData.RotationData.TargetRotationReachTime;
    }


    #endregion

    #region Input Methods

    // 退出移动状态 返回静止
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
