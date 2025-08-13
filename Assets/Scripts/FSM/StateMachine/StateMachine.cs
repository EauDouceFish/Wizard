using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    public abstract class StateMachine
    {
        protected IState currentState;

        public void ChangeState(IState newState)
        {
            currentState?.Exit();

            currentState = newState;

            currentState.Enter();

            OnStateChanged(currentState);
        }

        protected virtual void OnStateChanged(IState newState)
        {
            // A hook, default to do nothing.
        }

        public void HandleInput()
        {
            currentState.HandleInput();
        }

        public void Update()
        {
            currentState.Update();
        }

        public void PhysicsUpdate()
        {
            currentState.PhysicsUpdate();
        }

        public void OnAnimationEnterEvent()
        {
            currentState?.OnAnimationEnterEvent();
        }

        public void OnAnimationExitEvent()
        {
            currentState?.OnAnimationExitEvent();
        }

        public void OnAnimationTranslateEvent()
        {
            currentState?.OnAnimationTransitionEvent();
        }

        public void OnCastSpellAnimationEvent()
        {
            if (currentState is PlayerCastSpellState castSpellState)
            {
                castSpellState.ExecuteSpell();
            }
        }
    }

}


