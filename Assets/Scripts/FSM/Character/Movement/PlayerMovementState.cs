using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem
{
    public class PlayerMovementState : IState
    {

        protected PlayerMovementStateMachine stateMachine;

        protected PlayerMovementData movementData;

        public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
        {
            stateMachine = playerMovementStateMachine;
            movementData = stateMachine.Player.movementData;
        }

        #region IState Methods

        // IState�ӿ�ʵ�ֵķ����������˵�ǰ״̬�߼�

        /// <summary>
        /// ״̬������Enter���������InputActions�Ļص�
        /// </summary>
        public virtual void Enter()
        {
            //Debug.Log("State: " + GetType().Name);

            AddInputActionsCallbacks();
            // StartAnimation
        }

        public virtual void Exit()
        {
            RemoveInputActionsCallbacks();
            // StopAnimation
        }


        public virtual void HandleInput()
        {
            ReadMovementInput();
        }

        public virtual void Update()
        {
            HandleInput();
        }

        public virtual void PhysicsUpdate()
        {
             Move();
        }

        public virtual void OnAnimationEnterEvent()
        {
        }

        public virtual void OnAnimationExitEvent()
        {
        }

        /// <summary>
        /// ������Ȼ������Ҫ��Ȼ�Խӵ���һ��״̬
        /// </summary>
        public virtual void OnAnimationTransitionEvent()
        {
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// �����״̬������walk��Ϊ��ת��
        /// </summary>
        protected virtual void AddInputActionsCallbacks()
        {
            //stateMachine.Player.Input.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
        }


        protected virtual void RemoveInputActionsCallbacks()
        {
            //stateMachine.Player.Input.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
        }


        // ��ö�Ӧ�ƶ��ٶ�Ȼ�������ʵ��ֹͣ�ƶ�Ч��
        protected void DecelerateHorizontally()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

            // ����mass����,���м���
            stateMachine.Player.Rigidbody.AddForce
                (-playerHorizontalVelocity * stateMachine.ReusableData.MovementDecelerateForce, ForceMode.Acceleration);
        }

        #endregion

        #region Reusable Methods

        protected virtual void Move()
        {
            // �Ȳ鿴����Ƿ��������ٶȣ��������ú��ƶ�����
            if (stateMachine.ReusableData.MovementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifier == 0.0f)
            {
                return;
            }
            Vector3 movementDirection = GetMovementInputDirection();

            // ���ƶ�������ת���
            float targetRotationYAngle = Rotate(movementDirection);

            // ��������ҵ�Ŀ����ת�Ƕ�����һ���µ�ǰ������
            // ������ϵͳ����������泯������򣬽�����Ӧ������������ƶ�����
            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

            // ��ȡ����ƶ�����֮���ȡ�˶��ٶ�
            float movementSpeed = GetMovementSpeed();

            Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();

            // Unity�ĵ����Ƽ�ֱ�����ø����ٶȣ�����AddForce
            // ������velocity��˲ʱ�ģ�Addforce����һ��PhysicsUpdate�У���˳ʱ
            // ���������Ϸ��˵��AddForce����ͬʱ�ܵ������ʹ������ϵͳ������չ�Ը���
            stateMachine.Player.Rigidbody.AddForce(targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);

        }

        /// <summary>
        /// ��ת��ң��ȸ������ϣ���ĳ���Ȼ�������ת���ó���
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private float Rotate(Vector3 direction)
        {
            float directionAngle = UpdateTargetRotation(direction);

            RotateTowardsTargetRotation();

            return directionAngle;
        }


        /// <summary>
        /// ���ö���
        /// </summary>
        /// <param name="animationHash"></param>
        protected void StartAnimation(int animationHash)
        {
            stateMachine.Player.Animator.SetBool(animationHash, true);
        }

        /// <summary>
        /// ֹͣ����
        /// </summary>
        /// <param name="animationHash"></param>
        protected void StopAnimation(int animationHash)
        {
            stateMachine.Player.Animator.SetBool(animationHash, false);
        }

        // ��ȡƽ���ٶ�
        protected Vector3 GetPlayerHorizontalVelocity()
        {
            Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;
            //Debug.Log("GetHorizontalV: " + playerHorizontalVelocity);

            // ������ֱ����Ϊ0
            playerHorizontalVelocity.y = 0.0f;

            return playerHorizontalVelocity;
        }

        // ��ȡ��ֱ�ٶ�������yֵΪ�ٶ�
        protected Vector3 GetPlayerVerticalVelocity()
        {
            return new Vector3(0f, stateMachine.Player.Rigidbody.velocity.y, 0f);
        }

        /// <summary>
        /// ��ȡ�ƶ��ٶȣ��Լ��Ƿ���б�£����ʱ��Ҫ��
        /// </summary>
        /// <param name="shouldConsiderSlopes"></param>
        /// <returns></returns>
        protected float GetMovementSpeed(bool shouldConsiderSlopes = true)
        {
            float movementSpeed = movementData.BaseSpeed * stateMachine.ReusableData.MovementSpeedModifier * stateMachine.ReusableData.MovementSpeedBuffMultiplier;

            if (shouldConsiderSlopes)
            {
                movementSpeed *= stateMachine.ReusableData.MovementOnSlopesSpeedModifier;
            }

            return movementSpeed;
        }

        // ��ȡ�ƶ�����
        protected Vector3 GetMovementInputDirection()
        {
            return new Vector3(stateMachine.ReusableData.MovementInput.x, 0.0f, stateMachine.ReusableData.MovementInput.y);
        }

        // �����ƽ��ת��Ŀ�귽��
        protected void RotateTowardsTargetRotation()
        {
            float currentYAngle = stateMachine.GetPlayerRotationEular().y;

            if (currentYAngle == stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                return;
            }

            float smoothedYAngle = Mathf.SmoothDampAngle(
                currentYAngle, stateMachine.ReusableData.CurrentTargetRotation.y,
                ref stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y,
                stateMachine.ReusableData.TimeToReachTargetRotation.y - stateMachine.ReusableData.DampedTargetRotationPassedTime.y);

            stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

            Quaternion targetRotation = Quaternion.Euler(0.0f, smoothedYAngle, 0.0f);

            // ����ֱ�����õ���ʧЧ����
            // stateMachine.Player.transform.rotation = targetRotation;
            stateMachine.Player.Rigidbody.MoveRotation(targetRotation);

        }

        protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
        {
            float directionAngle = GetDirectionAngle(direction);

            if (directionAngle != stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                UpdateTargetRotationData(directionAngle);
            }

            return directionAngle;
        }

        protected Vector3 GetTargetRotationDirection(float targetAngle)
        {
            return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        private void UpdateTargetRotationData(float targetAngle)
        {
            stateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;

            stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0.0f;
        }

        protected void ResetVelocity()
        {
            stateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }

        // ���ô�ֱ����:ֻ����ˮƽ����
        protected void ResetVerticalVelocity()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
            stateMachine.Player.Rigidbody.velocity = playerHorizontalVelocity;
        }

        // ����ˮƽ����:ֻ���ִ�ֱ����
        protected void ResetHorizontalVelocity()
        {
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();
            stateMachine.Player.Rigidbody.velocity = playerVerticalVelocity;
        }

        /// <summary>
        /// �ж��Ƿ����ƶ�
        /// </summary>
        /// <param name="minimumMagnitude"></param>
        /// <returns></returns>
        protected bool IsMovingHorizontally(float minimumMagnitude = 0.1f)
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

            // ʹ��minimum���ƹ����������涶���ȵ��µ�΢С����
            Vector2 playerHorizontalMovement = new Vector2(playerHorizontalVelocity.x, playerHorizontalVelocity.z);

            // �����ֵ(����)���ʱ�򷵻��ж�����ˮƽ�ƶ�
            return playerHorizontalMovement.magnitude > minimumMagnitude;
        }


        // ����ŷ���ǵ�yawֵ
        private float GetDirectionAngle(Vector3 direction)
        {
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (directionAngle < 0.0f)
            {
                directionAngle += 360f;
            }

            return directionAngle;
        }

        #endregion

        #region Input Methods

        protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            // �������������л�����״̬
            //stateMachine.ReusableData.ShouldWalk = !stateMachine.ReusableData.ShouldWalk;
        }

        /// <summary>
        /// �˴���ʱ�ÿ�
        /// </summary>
        protected virtual void ReadMovementInput()
        {
            // ����е��Ŀ�꣬�����ƶ�������ΪMovementInput
            if (stateMachine.ReusableData.HasClickTarget)
            {
                Vector3 direction = GetClickMoveDirectionXZ();
                stateMachine.ReusableData.MovementInput = new Vector2(direction.x, direction.z);
            }
            else
            {
                // û�е��Ŀ��ʱ������ƶ�����
                stateMachine.ReusableData.MovementInput = Vector2.zero;
            }
        }

        // ��õ��λ�ã����������Ŀ����������XZƽ��ͶӰ�ĵ�λ������������
        private Vector3 GetClickMoveDirectionXZ()
        {
            Vector3 targetPosition = stateMachine.ReusableData.RightClickTargetPosition;
            Vector3 currentPosition = stateMachine.GetPlayerPosition();

            Vector3 direction = (targetPosition - currentPosition).normalized;
            direction.y = 0f;

            return direction;
        }

        #endregion
    }
}