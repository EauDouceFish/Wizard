using PlayerSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventTrigger : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = transform.parent.GetComponent<Player>();
    }

    /// <summary>
    /// ����MovementEnter�����¼�
    /// </summary>
    public void OnMovementEnterEvent()
    {
        player.OnAnimationEnterEvent();
    }
    /// <summary>
    /// ����MovementExit�����¼�
    /// </summary>
    public void OnMovementExitEvent()
    {
        // ���������ת�������У����ݲ㼶�ж��Ƿ���Դ��
        if (IsInAnimationTransition())
        {
            return;
        }
        player.OnAnimationExitEvent();
    }
    /// <summary>
    /// ����MovementTransition�����¼�
    /// </summary>
    public void OnMovementTranslateEvent()
    {
        player.OnAnimationTranslate();
    }

    public void OnCastSpellEvent()
    {
        player.OnCastSpellAnimationEvent();
    }

    /// <summary>
    /// ���ݲ㼶�ж��Ƿ���Դ�ϣ��㼶����Animator������
    /// </summary>
    /// <param name="layerIdx"></param>
    /// <returns></returns>
    private bool IsInAnimationTransition(int layerIdx = 0)
    {
        return player.Animator.IsInTransition(layerIdx);
    }
}
