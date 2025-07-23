using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystem
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerInputActions InputActions { get; private set; }
        public PlayerInputActions.PlayerActions PlayerActions { get; private set; }

        private void Awake()
        {
            InputActions = new PlayerInputActions();

            PlayerActions = InputActions.Player;
        }

        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }

        // ʹ��Э�̽�ֹһ��ʱ�������
        public void DisableActionFor(InputAction inputAction, float seconds)
        {
            StartCoroutine(DisableAction(inputAction, seconds));
        }

        private IEnumerator DisableAction(InputAction inputAction, float seconds)
        {
            inputAction.Disable();

            yield return new WaitForSeconds(seconds);

            inputAction.Enable();
        }
    }
}