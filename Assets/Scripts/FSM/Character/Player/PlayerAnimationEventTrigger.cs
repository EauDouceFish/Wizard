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
    /// 触发MovementEnter动画事件
    /// </summary>
    public void OnMovementEnterEvent()
    {
        player.OnAnimationEnterEvent();
    }
    /// <summary>
    /// 触发MovementExit动画事件
    /// </summary>
    public void OnMovementExitEvent()
    {
        // 如果动画在转换过程中，根据层级判断是否可以打断
        if (IsInAnimationTransition())
        {
            return;
        }
        player.OnAnimationExitEvent();
    }
    /// <summary>
    /// 触发MovementTransition动画事件
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
    /// 根据层级判断是否可以打断，层级根据Animator中设置
    /// </summary>
    /// <param name="layerIdx"></param>
    /// <returns></returns>
    private bool IsInAnimationTransition(int layerIdx = 0)
    {
        return player.Animator.IsInTransition(layerIdx);
    }
}
