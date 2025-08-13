using QFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : Actor, IBuffableEntity, IAttacker, IHasBuffSpeedMultiplier, IHasStatusUI
    {
        private PlayerModel playerModel;

        [SerializeField] public PlayerEntityData playerData;

        [SerializeField] public PlayerMovementData movementData;

        [SerializeField] public string currentMovementState;

        [SerializeField] public string currentCombatState;

        [SerializeField] public Transform enemy;

        [SerializeField] public Transform spellCastPos;

        [SerializeField] public LayerMask enemyLayerMask;

        [field: Header("Collisions")]
        [field: SerializeField] public PlayerCapsuleColliderUtility ColliderUtility { get; set; }

        [field: Header("Animations")]
        [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

        [field: SerializeField] public PlayerLayerData LayerData { get; private set; }


        public PlayerInput Input { get; private set; }

        public Rigidbody Rigidbody => rb;

        public Animator Animator => animator;

        public Transform MainCameraTransform { get; private set; }

        public PlayerMovementStateMachine movementStateMachine;

        private Camera mainCamera;

        private MagicInputController magicInputController;

        #region 战斗、Buff相关

        public BuffSystem<Entity> BuffSystem { get; private set; }

        public LayerMask AttackLayerMask { get => enemyLayerMask; set => enemyLayerMask = value; }

        // 还没使用框架BindableProperty时候的方案，后续可迭代统一逻辑
        public event Action<float> OnCurrentAttackChange;

        public float CurrentAttack
        {
            get { return playerModel.modelInited? playerModel.CurrentAttack.Value : playerData.playerBaseAttack; }
            set 
            {
                playerModel?.SetAttack(value);
                OnCurrentAttackChange.Invoke(value);
            }
        }


        public float BuffSpeedMultiplier { get => movementStateMachine.ReusableData.MovementSpeedBuffMultiplier; set { movementStateMachine.ReusableData.MovementSpeedBuffMultiplier = value; } }

        #endregion

        #region 生命周期

        private void Awake()
        {
            playerModel = this.GetModel<PlayerModel>();

            Input = GetComponent<PlayerInput>();
            // 用玩家gameObject初始化胶囊碰撞体特性
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimensions();
            AnimationData.Initialize();
            MainCameraTransform = Camera.main.transform;
            magicInputController = GetComponent<MagicInputController>();

            movementStateMachine = new PlayerMovementStateMachine(this);

            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
            movementStateMachine = new PlayerMovementStateMachine(this);
            BuffSystem = new BuffSystem<Entity>(this);
        }

        private void Start()
        {
            InitializeInputCallbacks();
            movementStateMachine.ChangeState(movementStateMachine.IdlingState);

            playerModel.SetPlayer(this);
        }

        protected override void Update()
        {
            base.Update();
            movementStateMachine.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            movementStateMachine.PhysicsUpdate();
            BuffSystem.FixedUpdate();
        }

        private void OnDestroy()
        {
            RemoveInputCallbacks();
        }

        // 同步胶囊体
        private void OnValidate()
        {
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimensions();
        }

        #endregion

        #region 输入系统初始化

        private void InitializeInputCallbacks()
        {
            if (Input?.PlayerActions != null)
            {
                // 右键移动
                Input.PlayerActions.RightClick.performed += OnRightClickPerformed;

                // E键交互
                Input.PlayerActions.Interact.performed += OnInteractPerformed;

                // 左键施法
                Input.PlayerActions.LeftClick.started += OnLeftClickStarted;
                Input.PlayerActions.LeftClick.canceled += OnLeftClickCanceled;

                // 暂停游戏
                Input.PlayerActions.Pause.performed += OnPausePerformed;
            }
        }

        private void RemoveInputCallbacks()
        {
            if (Input?.PlayerActions != null)
            {
                Input.PlayerActions.RightClick.performed -= OnRightClickPerformed;

                Input.PlayerActions.Interact.performed -= OnInteractPerformed;

                Input.PlayerActions.LeftClick.started -= OnLeftClickStarted;

                Input.PlayerActions.LeftClick.canceled -= OnLeftClickCanceled;

                Input.PlayerActions.Pause.performed -= OnPausePerformed;

            }
        }

        #endregion

        #region 输入处理


        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            //Debug.Log("Right Click Performed");
            MoveToClickPosition();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            //Debug.Log("E键，执行交互逻辑");
            PerformInteraction();
        }

        private void OnLeftClickStarted(InputAction.CallbackContext context)
        {
            if (magicInputController != null)
            {
                magicInputController.OnLeftClickStarted();
            }
        }

        private void OnLeftClickCanceled(InputAction.CallbackContext context)
        {
            if (magicInputController != null)
            {
                magicInputController.OnLeftClickCanceled();
            }
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            this.SendCommand<ToggleGamePauseCommand>();
        }

        #region 交互逻辑

        private void PerformInteraction()
        {
            // 在玩家周围寻找可交互对象
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, 2f);

            foreach (var collider in nearbyObjects)
            {
                // 检查是否有IInteractable接口
                var interactable = collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    Debug.Log($"与 {collider.name} 进行交互");
                    interactable.OnInteract(gameObject);
                    return;
                }

                // 检查是否有IInteractableEntity接口
                if (collider.TryGetComponent<InteractableEntity>(out var interactableEntity))
                {
                    Debug.Log($"与实体 {collider.name} 进行交互");
                    interactableEntity.OnInteract(gameObject);
                    return;
                }
            }

            Debug.Log("附近没有可交互的对象");
        }

        #endregion

        #endregion

        #region 移动Input逻辑

        private void MoveToClickPosition()
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("找不到主摄像机");
                return;
            }

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            int groundLayer = LayerMask.GetMask("Ground");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                SetDestination(hit.point);
            }
        }

        public void SetDestination(Vector3 destination)
        {
            if (movementStateMachine.ReusableData.IsChanneling) return;
            movementStateMachine.SetClickTarget(destination);
        }

        #endregion

        #region 玩家状态机

        public void OnAnimationTranslate()
        {
            movementStateMachine.OnAnimationTranslateEvent();
        }

        public void OnAnimationEnterEvent()
        {
            movementStateMachine.OnAnimationEnterEvent();
        }

        public void OnAnimationExitEvent()
        {
            movementStateMachine.OnAnimationExitEvent();
        }

        /// <summary>
        /// 施法动画事件 - 执行法术
        /// </summary>
        public void OnCastSpellAnimationEvent()
        {
            movementStateMachine.OnCastSpellAnimationEvent();
        }

        #endregion

        #region UI

        public void BindStatusUI(StatusUI statusUI)
        {

        }

        public void UpdateStatusUI(List<BuffBase<Entity>> buffs)
        {

        }

        #endregion

        #region GamePlay相关

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
        }   

        protected override void Dead()
        {
            this.SendEvent<OnPlayerDeadEvent>(new OnPlayerDeadEvent { });
        }


        #endregion
    }
}

public struct OnPlayerDeadEvent
{
}