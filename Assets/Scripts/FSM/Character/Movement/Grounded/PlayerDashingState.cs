using PlayerSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashingState : PlayerGroundedState
{
    private PlayerDashData dashData;
    private int consecutiveDashesUsed;

    private bool shouldKeepRotating;
    // ��̿�ʼ��ʱ��
    private float startTime;

    // ��ʼ��Ŀ�꣬�Լ�����Ƿ񳬹����յ�
    private Vector3 dashStartPosition;
    private Vector3 dashTargetPosition;
    private float dashDistance;
    private bool hasTargetPosition = false;

    public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        dashData = movementData.DashData;
    }

    #region IState Methods

    // ���ó���ٶ�
    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = dashData.SpeedModifier;

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.DashParameterHash);

        stateMachine.ReusableData.RotationData = dashData.RotationData;

        dashStartPosition = stateMachine.Player.transform.position;

        startTime = Time.time;

        Dash();

        // ������������������ת��
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
        // û�Ӷ�����ʱ��duration����
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

    // ˿���л���һ����������ֹ���߿����ܲ�
    public override void OnAnimationTransitionEvent()
    {
        // �����������ص���ֹ״̬
        base.OnAnimationTransitionEvent();
        CheckDestinationReached();
    }

    #endregion

    #region Main Methods

    // ����ֱ�ӴӾ�ֹ��ʼDash���߼����Ǿ�ֹ�򷵻�
    // ���Dash�ټ����ͻᵼ�¶�����һ�γ�̼���
    private void Dash()
    {
        Vector3 dashDirection = stateMachine.Player.transform.forward;

        dashDirection.y = 0;
        dashDirection = dashDirection.normalized;

        // Dashʱ��ת��˴�����Ҫ�ƶ����
        UpdateTargetRotation(dashDirection, false);

        CalculateDashTarget(dashDirection);

        CheckWillDashMoreThenDestinationAndReset();

        stateMachine.Player.Rigidbody.velocity = dashDirection * GetMovementSpeed(false);
    }

    // �������������Ƿ񳬹�Ŀ��λ�ã���������˻���뼱ͣ״̬��������run��ȥ
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
            Debug.Log("Dash���볬��Ŀ����룬�����趨����");
        }
    }

    private void CheckDestinationReached()
    {
        if (!hasTargetPosition)
        {
            HandleNoTarget();
            return;
        }

        // ����Ƿ�δ����Ŀ�꣬û�е��ͼ���run
        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.RunningState);
            Debug.Log("Dashδ����Ŀ�꣬����Running״̬");
        }
        else
        {
            //stateMachine.ChangeState(stateMachine.HardStoppingState); --���Ҫ��ͣ���Ժ���
            //stateMachine.ChangeState(stateMachine.MediumStoppingState);
            stateMachine.ChangeState(stateMachine.IdlingState);
            Debug.Log("Dash����Ŀ�꣬�л���Idling״̬");
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

            // ��ʹ����������������֮�����һ��ʱ�����ȴ
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

        // �κ������붼�ἤ�����ɵ����뻹δ�ͷ�
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
