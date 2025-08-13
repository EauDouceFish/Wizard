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

        #region ս����Buff���

        public BuffSystem<Entity> BuffSystem { get; private set; }

        public LayerMask AttackLayerMask { get => enemyLayerMask; set => enemyLayerMask = value; }

        // ��ûʹ�ÿ��BindablePropertyʱ��ķ����������ɵ���ͳһ�߼�
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

        #region ��������

        private void Awake()
        {
            playerModel = this.GetModel<PlayerModel>();

            Input = GetComponent<PlayerInput>();
            // �����gameObject��ʼ��������ײ������
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

        // ͬ��������
        private void OnValidate()
        {
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimensions();
        }

        #endregion

        #region ����ϵͳ��ʼ��

        private void InitializeInputCallbacks()
        {
            if (Input?.PlayerActions != null)
            {
                // �Ҽ��ƶ�
                Input.PlayerActions.RightClick.performed += OnRightClickPerformed;

                // E������
                Input.PlayerActions.Interact.performed += OnInteractPerformed;

                // ���ʩ��
                Input.PlayerActions.LeftClick.started += OnLeftClickStarted;
                Input.PlayerActions.LeftClick.canceled += OnLeftClickCanceled;

                // ��ͣ��Ϸ
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

        #region ���봦��


        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            //Debug.Log("Right Click Performed");
            MoveToClickPosition();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            //Debug.Log("E����ִ�н����߼�");
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

        #region �����߼�

        private void PerformInteraction()
        {
            // �������ΧѰ�ҿɽ�������
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, 2f);

            foreach (var collider in nearbyObjects)
            {
                // ����Ƿ���IInteractable�ӿ�
                var interactable = collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    Debug.Log($"�� {collider.name} ���н���");
                    interactable.OnInteract(gameObject);
                    return;
                }

                // ����Ƿ���IInteractableEntity�ӿ�
                if (collider.TryGetComponent<InteractableEntity>(out var interactableEntity))
                {
                    Debug.Log($"��ʵ�� {collider.name} ���н���");
                    interactableEntity.OnInteract(gameObject);
                    return;
                }
            }

            Debug.Log("����û�пɽ����Ķ���");
        }

        #endregion

        #endregion

        #region �ƶ�Input�߼�

        private void MoveToClickPosition()
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("�Ҳ����������");
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

        #region ���״̬��

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
        /// ʩ�������¼� - ִ�з���
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

        #region GamePlay���

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