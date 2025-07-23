using PlayerSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashingState : PlayerGroundedState
{
    private PlayerDashData dashData;
    private int consecutiveDashesUsed;

    private bool shouldKeepRotating;
    // 冲刺开始的时间
    private float startTime;

    // 起始、目标，以及检测是否超过了终点
    private Vector3 dashStartPosition;
    private Vector3 dashTargetPosition;
    private float dashDistance;
    private bool hasTargetPosition = false;

    public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        dashData = movementData.DashData;
    }

    #region IState Methods

    // 设置冲刺速度
    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = dashData.SpeedModifier;

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.DashParameterHash);

        stateMachine.ReusableData.RotationData = dashData.RotationData;

        dashStartPosition = stateMachine.Player.transform.position;

        startTime = Time.time;

        Dash();

        // 如果有输入则可以立刻转向
        shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;

        UpdateConsecutiveDashes();

    }

    public override void Exit()
    {
        base.Exit();

        StopAnimation(stateMachine.Player.AnimationData.DashParameterHash);
        SetBaseRotationData();
        hasTargetPosition = false;
    }

    public override void Update()
    {
        base.Update();
        // 没加动画暂时用duration来测
        if (Time.time - startTime >= dashData.Duration)
        {
            CheckDestinationReached();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (!shouldKeepRotating)
        {
            return;
        }

        RotateTowardsTargetRotation();
    }

    // 丝滑切换下一个动画：静止或者快速跑步
    public override void OnAnimationTransitionEvent()
    {
        // 如果无输入则回到静止状态
        base.OnAnimationTransitionEvent();
        CheckDestinationReached();
    }

    #endregion

    #region Main Methods

    // 处理直接从静止开始Dash的逻辑，非静止则返回
    // 如果Dash再加力就会导致额外多出一段冲刺加速
    private void Dash()
    {
        Vector3 dashDirection = stateMachine.Player.transform.forward;

        dashDirection.y = 0;
        dashDirection = dashDirection.normalized;

        // Dash时候转向此处不需要移动相机
        UpdateTargetRotation(dashDirection, false);

        CalculateDashTarget(dashDirection);

        CheckWillDashMoreThenDestinationAndReset();

        stateMachine.Player.Rigidbody.velocity = dashDirection * GetMovementSpeed(false);
    }

    // 不仅仅计算了是否超过目标位置，如果超过了会进入急停状态，而不是run回去
    private void CalculateDashTarget(Vector3 dashDirection)
    {
        float dashSpeed = GetMovementSpeed(false);
        dashDistance = dashSpeed * dashData.Duration;
        dashTargetPosition = dashStartPosition + dashDirection.normalized * dashDistance;
        hasTargetPosition = true;
    }

    private void CheckWillDashMoreThenDestinationAndReset()
    {
        Vector3 currentPosition = stateMachine.Player.transform.position;
        Vector3 remainDistance = stateMachine.ReusableData.ClickTargetPosition - currentPosition;
        remainDistance.y = 0;
        float remainDistanceMagnitude = remainDistance.magnitude;
        if (dashDistance > remainDistanceMagnitude)
        {
            stateMachine.ReusableData.ClickTargetPosition = GOExtensions.GetGroundPosition(dashTargetPosition);
            Debug.Log("Dash距离超过目标距离，重新设定距离");
        }
    }

    private void CheckDestinationReached()
    {
        if (!hasTargetPosition)
        {
            HandleNoTarget();
            return;
        }

        // 检查是否未到达目标，没有到就继续run
        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.RunningState);
            Debug.Log("Dash未到达目标，继续Running状态");
        }
        else
        {
            //stateMachine.ChangeState(stateMachine.HardStoppingState); --冲刺要急停，以后做
            //stateMachine.ChangeState(stateMachine.MediumStoppingState);
            stateMachine.ChangeState(stateMachine.IdlingState);
            Debug.Log("Dash到达目标，切换到Idling状态");
        }
    }

    private void HandleNoTarget()
    {
        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            //stateMachine.ChangeState(stateMachine.HardStoppingState);
            stateMachine.ChangeState(stateMachine.IdlingState);
            return;
        }
    }

    private void UpdateConsecutiveDashes()
    {
        if (IsConsecutive())
        {
            consecutiveDashesUsed = 0;
        }

        ++consecutiveDashesUsed;

        if (consecutiveDashesUsed == dashData.ConsecutiveDashesLimitAmount)
        {
            consecutiveDashesUsed = 0;

            // 在使用完可连续冲刺数量之后进入一段时间的冷却
            stateMachine.Player.Input.DisableActionFor(
                stateMachine.Player.Input.PlayerActions.Dash,
                dashData.DashLimitReachedCooldown
                );
        }
    }

    private bool IsConsecutive()
    {
        return Time.time < startTime + dashData.TimeToBeConsideredConsecutive;
    }

    #endregion

    #region Reusable Methods

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        // 任何新输入都会激活，即便旧的输入还未释放
        //stateMachine.Player.Input.PlayerActions.Movement.performed += OnMovementPerformed;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();

        //stateMachine.Player.Input.PlayerActions.Movement.performed += OnMovementPerformed;
    }

    #endregion

    #region Input Methods

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {

    }

    protected override void OnDashStarted(InputAction.CallbackContext context)
    {
    }

    #endregion
}
