using UnityEngine;

namespace PlayerSystem
{
    /// <summary>
    /// �����ݹ����࣬�ṩ���Ը���ֵ������
    /// </summary>
    public class PlayerStateReusableData
    {
        public Vector2 MovementInput { get; set; }
        public float MovementSpeedModifier { get; set; } = 1.0f;

        public float MovementSpeedBuffMultiplier { get; set; } = 1.0f;

        // б��ʱ�����΢����
        public float MovementOnSlopesSpeedModifier { get; set; } = 1.0f;

        // ֹͣʱ�����
        public float MovementDecelerateForce { get; set; } = 1.0f;

        // �Ƿ�����·
        public bool ShouldWalk { get; set; }

        // ����ƶ����
        public Vector3 RightClickTargetPosition { get; set; }
        public float ClickTargetReachDistance { get; set; } = 0.5f;//������������ʱ��ͣ��
        public bool HasClickTarget { get; set; }
        
        // ״̬�л�����
        public bool ShouldSwitchToCombat { get; set; }

        // ʩ�����
        public bool IsChanneling { get; set; }
        public bool IsInstantCast { get; set; }

        // ʩ��Ŀ��λ��
        public Vector3 CastTargetPosition { get; set; }
        public BasicSpellInstance CurrentChannelSpell { get; set; }


        // ��ǰ���ϣ���ĳ���
        private Vector3 currentTargetRotation;
        // ��ת����ʱ��:����ͨ���ֱ�洢
        private Vector3 timeToReachTargetRotation;
        // ������ת��ǰ�Ľ��ٶȣ�����ʵ��ƽ����ת
        private Vector3 dampedTargetRotationCurrentVelocity;
        // ��¼��ת��������ʱ�䣬����ʱ�����������ֵ
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

        // ��ǰ������ҵ���ת��Ϣ
        public PlayerRotationData RotationData { get; set; }
    }
}
