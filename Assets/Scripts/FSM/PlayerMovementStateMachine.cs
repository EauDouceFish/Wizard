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

    // ֻ��get������Ϊֻ������
    public Player Player { get; }

    public PlayerStateReusableData ReusableData { get; }

    // Grounded
    public PlayerIdlingState IdlingState { get; }
    public PlayerDashingState DashingState { get; }

    // - Moving���ֶ���δ���ƶ�����չ��
    //public PlayerWalkingState WalkingState { get; }
    public PlayerRunningState RunningState { get; }
    // - Stopping
    //public PlayerLightStoppingState LightStoppingState { get; }
    //public PlayerMediumStoppingState MediumStoppingState { get; }
    //public PlayerHardStoppingState HardStoppingState { get; }

    // ��״̬��ע�����п��ܵ�״̬
    public PlayerMovementStateMachine(Player player)
    {
        Player = player;

        ReusableData = new PlayerStateReusableData();

        IdlingState = new PlayerIdlingState(this);
        DashingState = new PlayerDashingState(this);
        //WalkingState = new PlayerWalkingState(this);
        RunningState = new PlayerRunningState(this);
        //NavigatingState = new PlayerNavigatingState(this); // PC�ˣ���������չ��3��

        //LightStoppingState = new PlayerLightStoppingState(this);
        //MediumStoppingState = new PlayerMediumStoppingState(this);
        //HardStoppingState = new PlayerHardStoppingState(this);
    }

    #region �ⲿ����
    /// <summary>
    /// �����������ķ���: Player.Rigidbody.AddForce()
    /// </summary>
    /// <param name="force"></param>
    /// <param name="forceMode"></param>
    public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
    {
        Player.Rigidbody.AddForce(force, forceMode);
    }

    /// <summary>
    /// �������ٶ�
    /// </summary>
    public Vector3 GetPlayerVelocity()
    {
        return Player.Rigidbody.velocity;
    }

    /// <summary>
    /// ��������ת
    /// </summary>
    public Quaternion GetPlayerRotation()
    {
        return Player.Rigidbody.rotation;
    }

    /// <summary>
    /// ������λ��
    /// </summary>
    public Vector3 GetPlayerPosition()
    {
        return Player.transform.position;
    }

    /// <summary>
    /// ��������ת--ŷ����
    /// </summary>
    public Vector3 GetPlayerRotationEular()
    {
        return Player.Rigidbody.rotation.eulerAngles;
    }

    /// <summary>
    /// ���õ���ƶ�Ŀ��
    /// </summary>
    /// <param name="targetPosition"></param>
    public void SetClickTarget(Vector3 targetPosition)
    {
        ReusableData.ClickTargetPosition = targetPosition;
        ReusableData.HasClickTarget = true;

        // �л���Running״̬
        ChangeState(RunningState);
    }

    /// <summary>
    /// �������ƶ�Ŀ��
    /// </summary>
    public void ClearClickTarget()
    {
        ReusableData.HasClickTarget = false;
        ReusableData.ClickTargetPosition = Vector3.zero;
    }

    /// <summary>
    /// ����Ƿ񵽴�Ŀ��λ��
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

