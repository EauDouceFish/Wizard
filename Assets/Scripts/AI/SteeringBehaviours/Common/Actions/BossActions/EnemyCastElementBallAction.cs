using System.Collections;
using UnityEngine;
using QFramework;
using BehaviorDesigner.Runtime.Tasks;
using PlayerSystem;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector3;

public class EnemyCastElementBallAction : EnemyAction, IController
{
    public float castTime = 0.7f;
    public float blindRange = 10f;
    
    private MagicSpellSystem magicSpellSystem;
    private Player player;
    

    private bool casted = false;
    private bool inBlindRange = false;

    public override void OnStart()
    {
        base.OnStart();

        // 获取系统和玩家引用
        magicSpellSystem = this.GetSystem<MagicSpellSystem>();
        player = this.GetModel<PlayerModel>().GetPlayer();

        if ((player.transform.position - enemy.transform.position).magnitude < blindRange)
        {
            inBlindRange = true;
        }
        else
        {
            StartCoroutine(CastBallWithDelay());
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (inBlindRange)
        {
            inBlindRange =false;
            return TaskStatus.Success;
        }
        if (casted != false)
        {
            casted = false;
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    private IEnumerator CastBallWithDelay()
    {
        Vector3 targetPosition = player.transform.position;

        yield return new WaitForSeconds(castTime - 0.1f);
        SetAnimationTrigger();
        yield return new WaitForSeconds(0.1f);
    
        float offset = GOExtensions.GetModelBoundsAABB(enemy.gameObject).size.y + 3f;
        BasicSpellInstance ballSpellInstance = magicSpellSystem.CreateBasicSpellInstanceByEnum(BasicSpellType.Ball);
        ballSpellInstance.Initialize(enemy, targetPosition, false, null, offset);
        ballSpellInstance.Execute();
        casted = true;
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}