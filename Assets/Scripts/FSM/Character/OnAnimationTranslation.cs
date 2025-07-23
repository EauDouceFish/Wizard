using UnityEngine;

namespace PlayerSystem
{
    public enum OnEnterAnimationPlayerState
    {
        Idle,               // ��ֹ
        Walk,               // ��·�������ã�
        Run,                // �ܲ�
        Dash,               // ��̣����ܣ�
        InstantCast,        // ˲��ʩ��
        ChannelCast,        // ����ʩ��
        ChantCast,          // ����ʩ��
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