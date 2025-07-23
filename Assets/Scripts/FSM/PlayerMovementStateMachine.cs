using PlayerSystem;
using QFramework;
using UnityEngine;
using static EventCenter;

public struct PlayerMovementStateChangedEvent
{
    public string NewStateName;
}

public class PlayerMovementStateMachine : StateMachine, ICanSendEvent
{
    protected override void OnStateChanged(IState newState)
    {
        base.OnStateChanged(newState);
        this.SendEvent(new PlayerMovementStateChangedEvent
        {
            NewStateName = newState.GetType().Name
        });
    }

    // 只用get，设置为只读属性
    public Player Player { get; }

    public PlayerStateReusableData ReusableData { get; }

    // Grounded
    public PlayerIdlingState IdlingState { get; }
    public PlayerDashingState DashingState { get; }

    // - Moving（手动，未来移动端拓展）
    //public PlayerWalkingState WalkingState { get; }
    public PlayerRunningState RunningState { get; }
    // - Stopping
    //public PlayerLightStoppingState LightStoppingState { get; }
    //public PlayerMediumStoppingState MediumStoppingState { get; }
    //public PlayerHardStoppingState HardStoppingState { get; }

    // 在状态机注册所有可能的状态
    public PlayerMovementStateMachine(Player player)
    {
        Player = player;

        ReusableData = new PlayerStateReusableData();

        IdlingState = new PlayerIdlingState(this);
        DashingState = new PlayerDashingState(this);
        //WalkingState = new PlayerWalkingState(this);
        RunningState = new PlayerRunningState(this);
        //NavigatingState = new PlayerNavigatingState(this); // PC端，后续可拓展到3种

        //LightStoppingState = new PlayerLightStoppingState(this);
        //MediumStoppingState = new PlayerMediumStoppingState(this);
        //HardStoppingState = new PlayerHardStoppingState(this);
    }

    #region 外部方法
    /// <summary>
    /// 给玩家添加力的方法: Player.Rigidbody.AddForce()
    /// </summary>
    /// <param name="force"></param>
    /// <param name="forceMode"></param>
    public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
    {
        Player.Rigidbody.AddForce(force, forceMode);
    }

    /// <summary>
    /// 获得玩家速度
    /// </summary>
    public Vector3 GetPlayerVelocity()
    {
        return Player.Rigidbody.velocity;
    }

    /// <summary>
    /// 获得玩家旋转
    /// </summary>
    public Quaternion GetPlayerRotation()
    {
        return Player.Rigidbody.rotation;
    }

    /// <summary>
    /// 获得玩家位置
    /// </summary>
    public Vector3 GetPlayerPosition()
    {
        return Player.transform.position;
    }

    /// <summary>
    /// 获得玩家旋转--欧拉角
    /// </summary>
    public Vector3 GetPlayerRotationEular()
    {
        return Player.Rigidbody.rotation.eulerAngles;
    }

    /// <summary>
    /// 设置点击移动目标
    /// </summary>
    /// <param name="targetPosition"></param>
    public void SetClickTarget(Vector3 targetPosition)
    {
        ReusableData.ClickTargetPosition = targetPosition;
        ReusableData.HasClickTarget = true;

        // 切换到Running状态
        ChangeState(RunningState);
    }

    /// <summary>
    /// 清除点击移动目标
    /// </summary>
    public void ClearClickTarget()
    {
        ReusableData.HasClickTarget = false;
        ReusableData.ClickTargetPosition = Vector3.zero;
    }

    /// <summary>
    /// 检查是否到达目标位置
    /// </summary>
    /// <returns></returns>
    public bool HasReachedClickTarget()
    {
        if (!ReusableData.HasClickTarget) return false;

        float distance = Vector3.Distance(GetPlayerPosition(), ReusableData.ClickTargetPosition);
        return distance <= ReusableData.ClickTargetReachDistance;
    }

    #endregion

    #region Architecture
    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
    #endregion
}

