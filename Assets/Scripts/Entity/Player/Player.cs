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

        #region ��������

        private void Awake()
        {
            Input = GetComponent<PlayerInput>();
            Rigidbody = GetComponent<Rigidbody>();
            Animator = GetComponentInChildren<Animator>(); // �ݹ飬���������ӽڵ�

            // �����gameObject��ʼ��������ײ������
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

        #region ���봦��


        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("Right Click Performed");
            MoveToClickPosition();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("E����ִ�н����߼�");
            PerformInteraction();
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

        #region �ƶ��߼�

        private void MoveToClickPosition()
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("�Ҳ����������");
                return;
            }

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                // ����Ƿ����ڿ��ƶ��ĵ�����
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



        #region ���״̬��

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