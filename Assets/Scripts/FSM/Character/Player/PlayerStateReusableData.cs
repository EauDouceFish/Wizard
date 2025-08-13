using UnityEngine;

namespace PlayerSystem
{
    /// <summary>
    /// 纯数据管理类，提供可以更改值的引用
    /// </summary>
    public class PlayerStateReusableData
    {
        public Vector2 MovementInput { get; set; }
        public float MovementSpeedModifier { get; set; } = 1.0f;

        public float MovementSpeedBuffMultiplier { get; set; } = 1.0f;

        // 斜坡时候会略微减速
        public float MovementOnSlopesSpeedModifier { get; set; } = 1.0f;

        // 停止时候减速
        public float MovementDecelerateForce { get; set; } = 1.0f;

        // 是否在走路
        public bool ShouldWalk { get; set; }

        // 点击移动相关
        public Vector3 RightClickTargetPosition { get; set; }
        public float ClickTargetReachDistance { get; set; } = 0.5f;//会在这个距离的时候停下
        public bool HasClickTarget { get; set; }
        
        // 状态切换控制
        public bool ShouldSwitchToCombat { get; set; }

        // 施法相关
        public bool IsChanneling { get; set; }
        public bool IsInstantCast { get; set; }

        // 施法目标位置
        public Vector3 CastTargetPosition { get; set; }
        public BasicSpellInstance CurrentChannelSpell { get; set; }


        // 当前玩家希望的朝向
        private Vector3 currentTargetRotation;
        // 旋转所需时间:几个通道分别存储
        private Vector3 timeToReachTargetRotation;
        // 阻尼旋转当前的角速度，用于实现平滑旋转
        private Vector3 dampedTargetRotationCurrentVelocity;
        // 记录旋转所经过的时间，与总时长结合来做插值
        private Vector3 dampedTargetRotationPassedTime;

        public ref Vector3 CurrentTargetRotation
        {
            get
            {
                return ref currentTargetRotation;
            }
        }

        public ref Vector3 TimeToReachTargetRotation
        {
            get
            {
                return ref timeToReachTargetRotation;
            }
        }
        public ref Vector3 DampedTargetRotationCurrentVelocity
        {
            get
            {
                return ref dampedTargetRotationCurrentVelocity;
            }
        }

        public ref Vector3 DampedTargetRotationPassedTime
        {
            get
            {
                return ref dampedTargetRotationPassedTime;
            }
        }

        // 当前控制玩家的旋转信息
        public PlayerRotationData RotationData { get; set; }
    }
}
