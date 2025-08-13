using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using PlayerSystem;

public abstract class BasicSpellInstance : SpellInstanceBase, ICanRegisterEvent
{
    protected BasicSpellBaseData spellData;

    protected Entity caster;
    protected LayerMask enemyLayerMask = -1;
    protected Vector3 targetPosition;


    protected Vector3 castDirection;
    protected List<MagicElement> castElements;
    protected bool isExecuted = false;

    /// <summary>
    /// 用于自定义修正不同敌人行为的施法位置
    /// </summary>
    protected float yOffset = 0f;

    public void SetSpellData(BasicSpellBaseData data)
    {
        spellData = data;
        SpellBaseData = data;
    }
    #region 生命周期方法

    protected virtual void Start()
    {
        this.RegisterEvent<OnChannelSpellCastEndedEvent>(e => TryStopChannelingSpell())
            .UnRegisterWhenGameObjectDestroyed(gameObject);

        if (caster is IAttacker attacker)
        {
            enemyLayerMask = attacker.AttackLayerMask;
        }
        else
        {
            Debug.LogWarning("caster需要实现IAttacker接口");
            enemyLayerMask = LayerMask.GetMask("Enemy");
        }
        if (spellData.isCastFromHand && caster is Player player)
        {
            transform.position = player.spellCastPos.position;
        }
    }


    protected virtual void Update()
    {
        // 对齐施法手部
        if (spellData.castType == CastType.Channel)
        {
            if (spellData.isCastFromHand && caster is Player player)
            {
                transform.position = player.spellCastPos.position;
            }
        }
    }

    #endregion

    public BasicSpellBaseData GetSpellData()
    {
        return spellData;
    }


    /// <summary>
    /// 传入caster，以及要施法的方位。最后一个参数可以选择是用direction决定，还是用targetPosition决定
    /// </summary>
    /// <param name="spellCaster"></param>
    /// <param name="targetParam"></param>
    /// <param name="elements"></param>
    /// <param name="yOffset"></param>
    public virtual void Initialize(Entity spellCaster, Vector3 targetParam, bool useDirection, List<MagicElement> elements = null, float yOffset = 0)
    {
        caster = spellCaster;
        targetPosition = targetParam;
        castElements = new List<MagicElement>(elements ?? new List<MagicElement>());
        this.yOffset = yOffset;
        if (spellData.isCastFromHand && caster is Player)
        {
            Player player = caster as Player;
            transform.position =  player.spellCastPos.position;
        }
        else
        {
            transform.position = caster.transform.position + Vector3.up * yOffset;
        }

        CalculateCastDirection(useDirection);

        SetSpellRotation();


        //Debug.Log($"法术实例已初始化: {GetType().Name}, 方向: {castDirection}, 颜色：{spellData.spellColor}");
    }

    public virtual void Execute()
    {
        if (isExecuted) return;
        isExecuted = true;
        ExecuteSpell();
    }

    private float ProcessStatusDamageLogic(IBuffableEntity target, List<MagicElement> spellElementList)
    {
        MagicSpellModel spellModel = this.GetModel<MagicSpellModel>();
        HashSet<MagicElement> spellElements = new HashSet<MagicElement>(spellElementList);
        float multiplier = 1f;
        bool spellHasReacted = false;

        // 1. 先除去Nature、Rock，检测法术本身是否有反应
        bool hasFire = spellElements.Contains(MagicElement.Fire);
        bool hasWater = spellElements.Contains(MagicElement.Water);
        bool hasIce = spellElements.Contains(MagicElement.Ice);
        bool hasNature = spellElements.Contains(MagicElement.Nature);
        bool hasRock = spellElements.Contains(MagicElement.Rock);

        // 法术自身的反应增伤，后续可以改成配置逻辑
        // 火+冰
        if (hasFire && hasIce)
        {
            multiplier *= spellModel.FireIceReactionMultiplierInSpell;
            spellHasReacted = true;
        }
        // 火+水
        else if (hasFire && hasWater)
        {
            multiplier *= spellModel.FireWaterReactionMultiplierInSpell;
            spellHasReacted = true;
        }
        // 冰+水
        else if (hasIce && hasWater)
        {
            // 给对方挂减速Buff
            multiplier *= spellModel.IceWaterReactionMultiplierInSpell;
            spellHasReacted = true;
            target.BuffSystem.AddBuff<SlowDown>(caster.name, 1);
        }
        // 法术自身没反应，检测目标身上的元素是否可反应。这部分逻辑后续需要优化
        // 预计通过BuffTag迭代实现相关反应逻辑，此处快速开发
        else
        {
            List<BuffBase<Entity>> targetStatusBuffs = target.BuffSystem.FindAllBuff();
            if (hasFire)
            {
                if (targetStatusBuffs.Exists(buff => buff is IceStatus))
                {
                    multiplier *= spellModel.FireIceReactionMultiplier;
                    spellHasReacted = true;
                    var iceBuff = target.BuffSystem.FindBuff<IceStatus>();
                    if (iceBuff.BuffData.isPermanent == false)
                    {
                        target.BuffSystem.RemoveBuff<IceStatus>();
                    }
                }
                else if (targetStatusBuffs.Exists(buff => buff is WaterStatus))
                {
                    multiplier *= spellModel.FireWaterReactionMultiplier;
                    spellHasReacted = true;
                    var waterBuff = target.BuffSystem.FindBuff<WaterStatus>();
                    if (waterBuff.BuffData.isPermanent == false)
                    {
                        //Debug.Log("移除目标的Buff");
                        target.BuffSystem.RemoveBuff<WaterStatus>();
                    }
                }
            }
            else if (hasWater)
            {
                if (targetStatusBuffs.Exists(buff => buff is FireStatus))
                {
                    multiplier *= spellModel.FireWaterReactionMultiplier;
                    spellHasReacted = true;

                    var fireBuff = target.BuffSystem.FindBuff<FireStatus>();
                    if (fireBuff.BuffData.isPermanent == false)
                    {
                        //Debug.Log("移除目标的Buff");
                        target.BuffSystem.RemoveBuff<FireStatus>();
                    }
                }
                else if (targetStatusBuffs.Exists(buff => buff is IceStatus))
                {
                    multiplier *= spellModel.IceWaterReactionMultiplier;
                    spellHasReacted = true;
                    target.BuffSystem.AddBuff<SlowDown>(caster.name, 1);
                    var iceBuff = target.BuffSystem.FindBuff<IceStatus>();
                    if (iceBuff.BuffData.isPermanent == false)
                    {
                        target.BuffSystem.RemoveBuff<IceStatus>();
                    }
                }
            }
            else if (hasIce)
            {
                if (targetStatusBuffs.Exists(buff => buff is WaterStatus))
                {
                    // 伤害不变，给对方挂减速Buff
                    multiplier *= spellModel.IceWaterReactionMultiplier;
                    spellHasReacted = true;
                    target.BuffSystem.AddBuff<SlowDown>(caster.name, 1);
                    var waterBuff = target.BuffSystem.FindBuff<WaterStatus>();
                    if (waterBuff.BuffData.isPermanent == false)
                    {
                        target.BuffSystem.RemoveBuff<WaterStatus>();
                    }
                }
                else if (targetStatusBuffs.Exists(buff => buff is FireStatus))
                {
                    multiplier *= spellModel.FireIceReactionMultiplier;
                    spellHasReacted = true;
                    var fireBuff = target.BuffSystem.FindBuff<FireStatus>();
                    if (fireBuff.BuffData.isPermanent == false)
                    {
                        target.BuffSystem.RemoveBuff<FireStatus>();
                    }
                }
            }
            else if // 自然系反应
                ((hasNature && targetStatusBuffs.Exists(buff => buff is RockStatus)) ||
                (hasRock && targetStatusBuffs.Exists(buff => buff is NatureStatus)))
            {
                multiplier *= spellModel.NatureRockReactionMultiplier;
                spellHasReacted = true;
                if (hasNature)
                {
                    var rockBuff = target.BuffSystem.FindBuff<RockStatus>();
                    if (rockBuff != null && rockBuff.BuffData.isPermanent == false)
                    {
                        target.BuffSystem.RemoveBuff<RockStatus>();
                    }
                }
                else if (hasRock)
                {
                    var natureBuff = target.BuffSystem.FindBuff<NatureStatus>();
                    if (natureBuff != null && natureBuff.BuffData.isPermanent == false)
                    {
                        target.BuffSystem.RemoveBuff<NatureStatus>();
                    }
                }
            }

        }

        // 如果没有反应，挂元素Buff
        if (!spellHasReacted)
        {
            foreach (var element in spellElements)
            {
                switch (element)
                {
                    case MagicElement.Fire:
                        target.BuffSystem.AddBuff<FireStatus>(caster.name, 1);
                        break;
                    case MagicElement.Water:
                        target.BuffSystem.AddBuff<WaterStatus>(caster.name, 1);
                        break;
                    case MagicElement.Ice:
                        target.BuffSystem.AddBuff<IceStatus>(caster.name, 1);
                        break;
                    case MagicElement.Nature:
                        target.BuffSystem.AddBuff<NatureStatus>(caster.name, 1);
                        break;
                    case MagicElement.Rock:
                        target.BuffSystem.AddBuff<RockStatus>(caster.name, 1);
                        break;
                }
            }
        }
        if (spellHasReacted)
        {
            Debug.Log($"法术 {spellData.name} 与目标发生了元素反应，增伤倍率为 {multiplier}");
        }
        return multiplier;
    }

    /// <summary>
    /// 默认围绕caster位置旋转
    /// </summary>
    protected virtual void CalculateCastDirection(bool useDirection)
    {
        Player player = caster as Player;
        if (player != null)
        {
            castDirection = player.movementStateMachine.ReusableData.CastTargetPosition - player.transform.position;
            castDirection.y = 0f;
            castDirection.Normalize();
        }
        else if(!useDirection)
        {
            // 不是玩家，则往传入的目标方向发射
            castDirection = targetPosition - caster.transform.position;
            castDirection.y = 0f;
            castDirection.Normalize();
        }
        else
        {
            castDirection = caster.transform.forward;
        }
    }

    protected virtual void SetSpellRotation()
    {
        if (castDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(castDirection);
        }
    }

    protected virtual void PlaySpellEffect(Vector3 position)
    {
        if (spellData.spellPrefab != null)
        {
            transform.rotation = Quaternion.LookRotation(castDirection);

            GameObject effect = Instantiate(spellData.spellPrefab, position + Vector3.up * yOffset, transform.rotation);

            VFXColorHelper.ApplyColorToVFX(effect, spellData.spellColor);

            effect.transform.SetParent(transform);
            // Channel类型法术自动销毁
            //if(spellData.castType != CastType.Channel) Destroy(effect, 5f);
            if (spellData.castType != CastType.Channel)
            {
                Destroy(effect, 5f);
            }
        }
    }

    protected virtual Vector3 GetAdjustedTargetPosition(float distance)
    {
        return transform.position + castDirection * distance;
    }

    public Vector3 GetCastDirection()
    {
        return castDirection;
    }

    /// <summary>
    /// Channel法术，会随着输入位置变化，更新目标方位
    /// </summary>
    /// <param name="newTargetPosition"></param>
    public void UpdateChannelTarget(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
        CalculateCastDirection(true);
        SetSpellRotation();
    }

    protected Vector3 GetCastPos()
    {
        Vector3 pos;
        if (spellData.isCastFromHand && caster is Player player)
        {
            pos = player.spellCastPos.position;
        }
        else
        {
            pos = caster.transform.position + Vector3.up * yOffset;
        }
        return pos;
    }

    protected abstract void ExecuteSpell();

    #region Reusable Methods
    protected void DealDamage(Entity target)
    {
        if (target == null || target == caster) return;
        float spellAttackDamage = CalculateSpellAttack();
        Debug.Log($"咒语伤害强度：SpellAttack: {spellAttackDamage}");

        if (target is IBuffableEntity buffableTarget)
        {
            float reactionMultiplier = ProcessStatusDamageLogic(buffableTarget, castElements);
            Debug.Log($"反应倍率 MultiPlier: {reactionMultiplier}");
            spellAttackDamage *= reactionMultiplier;
        }

        Debug.Log($"最终伤害= {spellAttackDamage}");
        target.TakeDamage(spellAttackDamage);

        //Debug.Log($"对 {target.name} 造成 {finalDamage} 点伤害");
    }

    /// <summary>
    /// 计算逻辑是，咒语伤害=基础伤害+攻击者攻击力（后续还要加上反应伤害）
    /// </summary>
    protected virtual float CalculateSpellAttack()
    {
        float damage = spellData.baseDamage;

        if (caster is IAttacker attacker)
        {
            damage += attacker.CurrentAttack;
        }

        damage *= spellData.damageInterval;

        return damage;
    }


    /// <summary>
    /// 获取范围内的有效目标
    /// </summary>
    protected virtual List<Entity> GetValidTargets(Collider[] colliders)
    {
        List<Entity> targets = new List<Entity>();

        foreach (var collider in colliders)
        {
            Entity entity = collider.GetComponent<Entity>();
            if (entity != null && entity != caster)
            {
                targets.Add(entity);
            }
        }
        return targets;
    }

    // 计算角度（带有y分量）
    protected virtual bool IsInAngleRange(Vector3 direction, Vector3 toTarget, float maxAngle)
    {
        float angle = Vector3.Angle(direction, toTarget);
        return angle <= maxAngle / 2f;
    }

    // 计算水平角度（忽略y分量），Spray
    protected virtual bool IsInHorizontalAngleRange(Vector3 direction, Vector3 toTarget, float maxAngle)
    {
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;
        Vector3 horizontalToTarget = new Vector3(toTarget.x, 0, toTarget.z).normalized;

        float angle = Vector3.Angle(horizontalDirection, horizontalToTarget);
        return angle <= maxAngle / 2f;
    }

    // 计算高度能否打到
    protected virtual bool IsInVerticalRange(Vector3 targetPosition)
    {
        float heightDifference = Mathf.Abs(targetPosition.y - transform.position.y);
        return heightDifference <= spellData.spellHeight;
    }

    public void TryStopChannelingSpell()
    {
        if (spellData.castType == CastType.Channel)
        {
            DestroySelf();
        }
    }

    private IEnumerator RecycleEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPool.Recycle(effect);
    }

    // 后期考虑用对象池优化
    public void DestroySelf()
    {
        //ObjectPool.Recycle(gameObject);
        Destroy(gameObject);
    }

    #endregion
}
