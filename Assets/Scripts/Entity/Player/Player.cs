using PlayerSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace PlayerSystem
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : Entity, IBuffableEntity
    {
        [SerializeField] PlayerEntityData playerData;

        [SerializeField] public PlayerMovementData movementData;

        [SerializeField] public string currentMovementState;

        [SerializeField] public string currentCombatState;

        [SerializeField] public Transform enemy;

        [field: Header("Collisions")]
        [field: SerializeField] public PlayerCapsuleColliderUtility ColliderUtility { get; set; }

        [field: Header("Animations")]
        [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

        [field: SerializeField] public PlayerLayerData LayerData { get; private set; }


        public PlayerInput Input { get; private set; }

        public Rigidbody Rigidbody { get; private set; }

        public Animator Animator { get; private set; }

        public Transform MainCameraTransform { get; private set; }

        private PlayerMovementStateMachine movementStateMachine;

        private PlayerCombatStateMachine combatStateMachine;

        private Camera mainCamera;


        #region Buff System

        public BuffSystem<Entity> BuffSystem { get; private set; }

        public HashSet<EntityBuffBase> Buffs { get; private set; } = new();

        public void AddBuff(EntityBuffBase buff)
        {
            Buffs.Add(buff);
        }

        public void RemoveBuff(EntityBuffBase buff)
        {
            Buffs.Remove(buff);
        }

        #endregion

        #region 生命周期

        private void Awake()
        {
            Input = GetComponent<PlayerInput>();
            Rigidbody = GetComponent<Rigidbody>();
            Animator = GetComponentInChildren<Animator>(); // 递归，先自身，后子节点

            // 用玩家gameObject初始化胶囊碰撞体特性
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimensions();
            AnimationData.Initialize();
            MainCameraTransform = Camera.main.transform;

            movementStateMachine = new PlayerMovementStateMachine(this);

            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
            movementStateMachine = new PlayerMovementStateMachine(this);

        }

        private void Start()
        {
            InitializeInputCallbacks();
            movementStateMachine.ChangeState(movementStateMachine.IdlingState);
        }

        private void Update()
        {
            movementStateMachine.Update();
        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
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
            }
        }

        private void RemoveInputCallbacks()
        {
            if (Input?.PlayerActions != null)
            {
                Input.PlayerActions.RightClick.performed -= OnRightClickPerformed;

                Input.PlayerActions.Interact.performed -= OnInteractPerformed;
            }
        }

        #endregion

        #region 输入处理


        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("Right Click Performed");
            MoveToClickPosition();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("E键，执行交互逻辑");
            PerformInteraction();
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

        #region 移动逻辑

        private void MoveToClickPosition()
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("找不到主摄像机");
                return;
            }

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                // 检查是否点击在可移动的地面上
                if (hit.collider.CompareTag("Ground") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    SetDestination(hit.point);
                }
            }
        }

        public void SetDestination(Vector3 destination)
        {
            movementStateMachine.SetClickTarget(destination);
        }

        #endregion



        #region 玩家状态机

        public void OnAnimationTranslate
            ()
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

            combatStateMachine.OnAnimationExitEvent();
        }

        #endregion

    }
}