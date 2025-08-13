using UnityEngine;

public class PlayerCastSpellState : PlayerGroundedState
{
    public PlayerCastSpellState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    #region IState Methods

    // ʩ��״̬����޷��ƶ���������ת
    public override void Enter()
    {
        stateMachine.ReusableData.MovementSpeedModifier = 0.0f;

        RotateToTargetPosition();

        StartAnimation(stateMachine.Player.AnimationData.CastSpellParameterHash);

        ResetVelocity();

        stateMachine.ClearClickTarget();
    }

    public override void Exit()
    {
        StopAnimation(stateMachine.Player.AnimationData.CastSpellParameterHash);

        // ����ʩ��״̬����
        ClearCastingData();
    }

    public override void Update()
    {
        base.Update();

        if (ShouldExitCastState())
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
        }
    }

    // ��ʩ��״̬�´�����ת
    public override void PhysicsUpdate()
    {
        // ������ʩ��ʱ������귽����ת
        if (stateMachine.ReusableData.IsChanneling)
        {
            HandleMouseDirectionRotation();
        }

        if (IsMovingHorizontally())
        {
            ResetVelocity();
        }
        FloatCapsule();
    }

    public override void OnAnimationExitEvent()
    {
        base.OnAnimationExitEvent();

        // ˲��ʩ������������ɣ����ʩ�����
        if (stateMachine.ReusableData.IsInstantCast)
        {
            MarkSpellCompleted();
        }
    }

    #endregion

    #region Cast Spell Methods

    //����ʩ��������˲��ʩ�����˳��߼���ͬ
    private bool ShouldExitCastState()
    {
        var reusableData = stateMachine.ReusableData;

        // ����ʩ��������������ʱ�˳�
        if (!reusableData.IsInstantCast && !reusableData.IsChanneling)
        {
            return true;
        }

        // ˲��ʩ������������ִ�����ʱ�˳�
        if (reusableData.IsInstantCast && reusableData.CurrentChannelSpell == null)
        {
            return true;
        }

        return false;
    }

    private void MarkSpellCompleted()
    {
        stateMachine.ReusableData.CurrentChannelSpell = null;
    }

    private void ClearCastingData()
    {
        var reusableData = stateMachine.ReusableData;
        reusableData.IsChanneling = false;
        reusableData.IsInstantCast = false;
        reusableData.CastTargetPosition = Vector3.zero;
        reusableData.CurrentChannelSpell = null;
    }


    private void RotateToTargetPosition()
    {
        if (stateMachine.ReusableData.CastTargetPosition == Vector3.zero)
        {
            return;
        }

        Vector3 currentPosition = stateMachine.GetPlayerPosition();
        Vector3 targetPosition = stateMachine.ReusableData.CastTargetPosition;

        Vector3 direction = (targetPosition - currentPosition).normalized;
        direction.y = 0f;

        if (direction == Vector3.zero)
        {
            return;
        }

        UpdateTargetRotation(direction, false);

        float targetAngle = GetDirectionAngle(direction);
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        stateMachine.Player.Rigidbody.MoveRotation(targetRotation);
    }

    // ����ʱ����Һ���Ч����Ҫ������귽��
    private void HandleMouseDirectionRotation()
    {
        Vector3 mouseWorldPosition = GOExtensions.GetMouseWorldPositionOnGround();

        if (mouseWorldPosition == Vector3.zero)
        {
            return;
        }

        Vector3 currentPosition = stateMachine.GetPlayerPosition();
        Vector3 direction = (mouseWorldPosition - currentPosition).normalized;
        direction.y = 0f;

        if (direction == Vector3.zero)
        {
            return;
        }

        UpdateTargetRotation(direction, false);

        RotateTowardsTargetRotation();

        stateMachine.ReusableData.CastTargetPosition = mouseWorldPosition;

        // ����ʩ����Ч�ķ���
        if (stateMachine.ReusableData.IsChanneling && stateMachine.ReusableData.CurrentChannelSpell != null)
        {
            stateMachine.ReusableData.CurrentChannelSpell.UpdateChannelTarget(stateMachine.Player.transform.forward);
        }
        else
        {
            Debug.Log($"{stateMachine.ReusableData.IsChanneling}, {stateMachine.ReusableData.CurrentChannelSpell != null}");
        }
    }


    private float GetDirectionAngle(Vector3 direction)
    {
        float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        if (directionAngle < 0.0f)
        {
            directionAngle += 360f;
        }

        return directionAngle;
    }

    public void ExecuteSpell()
    {
        var currentSpell = stateMachine.ReusableData.CurrentChannelSpell;
        if (currentSpell != null)
        {
            Debug.Log($"ִ�з���: {currentSpell.GetType().Name}");
            currentSpell.Execute();

            // ˲��ʩ��ִ�к�����������
            if (stateMachine.ReusableData.IsInstantCast)
            {
                MarkSpellCompleted();
            }
        }
    }
    #endregion
}


public struct OnChannelSpellCastEndedEvent{

}