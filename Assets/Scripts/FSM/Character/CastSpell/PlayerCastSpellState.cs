using UnityEngine;

public class PlayerCastSpellState : PlayerGroundedState
{
    public PlayerCastSpellState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    #region IState Methods

    // 施法状态玩家无法移动但可以旋转
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

        // 清理施法状态数据
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

    // 在施法状态下处理旋转
    public override void PhysicsUpdate()
    {
        // 在蓄力施法时跟随鼠标方向旋转
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

        // 瞬发施法动画播放完成，标记施法完成
        if (stateMachine.ReusableData.IsInstantCast)
        {
            MarkSpellCompleted();
        }
    }

    #endregion

    #region Cast Spell Methods

    //蓄力施法，还有瞬发施法的退出逻辑不同
    private bool ShouldExitCastState()
    {
        var reusableData = stateMachine.ReusableData;

        // 蓄力施法：当不再蓄力时退出
        if (!reusableData.IsInstantCast && !reusableData.IsChanneling)
        {
            return true;
        }

        // 瞬发施法：当法术已执行完成时退出
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

    // 蓄力时，玩家和特效方向要跟随鼠标方向
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

        // 更新施法特效的方向
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
            Debug.Log($"执行法术: {currentSpell.GetType().Name}");
            currentSpell.Execute();

            // 瞬发施法执行后立即标记完成
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