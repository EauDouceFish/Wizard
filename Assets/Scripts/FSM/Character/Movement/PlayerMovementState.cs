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

        // IState接口实现的方法，定义了当前状态逻辑

        /// <summary>
        /// 状态机基类Enter方法，添加InputActions的回调
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
        /// 动画自然结束后：要自然对接的下一个状态
        /// </summary>
        public virtual void OnAnimationTransitionEvent()
        {
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// 基类的状态机控制walk行为的转换
        /// </summary>
        protected virtual void AddInputActionsCallbacks()
        {
            //stateMachine.Player.Input.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
        }


        protected virtual void RemoveInputActionsCallbacks()
        {
            //stateMachine.Player.Input.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
        }


        // 获得对应移动速度然后减掉，实现停止移动效果
        protected void DecelerateHorizontally()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

            // 忽略mass属性,进行减速
            stateMachine.Player.Rigidbody.AddForce
                (-playerHorizontalVelocity * stateMachine.ReusableData.MovementDecelerateForce, ForceMode.Acceleration);
        }

        #endregion

        #region Reusable Methods

        protected virtual void Move()
        {
            // 先查看玩家是否有输入速度，有则设置好移动方向
            if (stateMachine.ReusableData.MovementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifier == 0.0f)
            {
                return;
            }
            Vector3 movementDirection = GetMovementInputDirection();

            // 向移动方向旋转玩家
            float targetRotationYAngle = Rotate(movementDirection);

            // 来根据玩家的目标旋转角度生成一个新的前进向量
            // 即告诉系统“玩家现在面朝这个方向，接下来应该往这个方向移动”。
            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

            // 获取玩家移动方向之后获取运动速度
            float movementSpeed = GetMovementSpeed();

            Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();

            // Unity文档不推荐直接设置刚体速度，而是AddForce
            // 但设置velocity是瞬时的，Addforce在下一个PhysicsUpdate中，不顺时
            // 对于这个游戏来说，AddForce可以同时受到多个力使用物理系统，可拓展性更好
            stateMachine.Player.Rigidbody.AddForce(targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);

        }

        /// <summary>
        /// 旋转玩家，先更新玩家希望的朝向，然后将玩家旋转到该朝向
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
        /// 启用动画
        /// </summary>
        /// <param name="animationHash"></param>
        protected void StartAnimation(int animationHash)
        {
            stateMachine.Player.Animator.SetBool(animationHash, true);
        }

        /// <summary>
        /// 停止动画
        /// </summary>
        /// <param name="animationHash"></param>
        protected void StopAnimation(int animationHash)
        {
            stateMachine.Player.Animator.SetBool(animationHash, false);
        }

        // 获取平行速度
        protected Vector3 GetPlayerHorizontalVelocity()
        {
            Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;
            //Debug.Log("GetHorizontalV: " + playerHorizontalVelocity);

            // 修正竖直分量为0
            playerHorizontalVelocity.y = 0.0f;

            return playerHorizontalVelocity;
        }

        // 获取垂直速度向量，y值为速度
        protected Vector3 GetPlayerVerticalVelocity()
        {
            return new Vector3(0f, stateMachine.Player.Rigidbody.velocity.y, 0f);
        }

        /// <summary>
        /// 获取移动速度，以及是否考虑斜坡（冲刺时不要）
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

        // 获取移动方向
        protected Vector3 GetMovementInputDirection()
        {
            return new Vector3(stateMachine.ReusableData.MovementInput.x, 0.0f, stateMachine.ReusableData.MovementInput.y);
        }

        // 让玩家平滑转向到目标方向
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

            // 这里直接设置导致失效！！
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

        // 重置垂直分量:只保持水平分量
        protected void ResetVerticalVelocity()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
            stateMachine.Player.Rigidbody.velocity = playerHorizontalVelocity;
        }

        // 重置水平分量:只保持垂直分量
        protected void ResetHorizontalVelocity()
        {
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();
            stateMachine.Player.Rigidbody.velocity = playerVerticalVelocity;
        }

        /// <summary>
        /// 判断是否有移动
        /// </summary>
        /// <param name="minimumMagnitude"></param>
        /// <returns></returns>
        protected bool IsMovingHorizontally(float minimumMagnitude = 0.1f)
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

            // 使用minimum限制过滤物理引擎抖动等导致的微小噪音
            Vector2 playerHorizontalMovement = new Vector2(playerHorizontalVelocity.x, playerHorizontalVelocity.z);

            // 比最低值(噪音)大的时候返回判断正在水平移动
            return playerHorizontalMovement.magnitude > minimumMagnitude;
        }


        // 计算欧拉角的yaw值
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
            // 后续尝试制作切换行走状态
            //stateMachine.ReusableData.ShouldWalk = !stateMachine.ReusableData.ShouldWalk;
        }

        /// <summary>
        /// 此处暂时置空
        /// </summary>
        protected virtual void ReadMovementInput()
        {
            // 如果有点击目标，计算移动方向作为MovementInput
            if (stateMachine.ReusableData.HasClickTarget)
            {
                Vector3 direction = GetClickMoveDirectionXZ();
                stateMachine.ReusableData.MovementInput = new Vector2(direction.x, direction.z);
            }
            else
            {
                // 没有点击目标时，清空移动输入
                stateMachine.ReusableData.MovementInput = Vector2.zero;
            }
        }

        // 获得点击位置，和玩家连线目标向量，在XZ平面投影的单位向量（纯方向）
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