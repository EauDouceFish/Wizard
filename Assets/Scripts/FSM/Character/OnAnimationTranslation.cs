using UnityEngine;

namespace PlayerSystem
{
    public enum OnEnterAnimationPlayerState
    {
        Idle,               // 静止
        Walk,               // 走路（不常用）
        Run,                // 跑步
        Dash,               // 冲刺（闪避）
        InstantCast,        // 瞬发施法
        ChannelCast,        // 蓄力施法
        ChantCast,          // 吟唱施法
        Null
    }

    public class OnAnimationTranslation : StateMachineBehaviour
    {
        Player player;

        [SerializeField] public OnEnterAnimationPlayerState onEnterAnimationState;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onEnterAnimationState == OnEnterAnimationPlayerState.Null)
            {
                return;
            }
            if (animator.TryGetComponent<Player>(out player))
            {
                //player.OnAnimationTranslate
                //(onEnterAnimationState);
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.TryGetComponent<Player>(out player))
            {
                //player.OnAnimationExitEvent();
            }
        }
    }

}