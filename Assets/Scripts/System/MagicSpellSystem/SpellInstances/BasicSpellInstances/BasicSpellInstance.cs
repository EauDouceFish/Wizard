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
    /// �����Զ���������ͬ������Ϊ��ʩ��λ��
    /// </summary>
    protected float yOffset = 0f;

    public void SetSpellData(BasicSpellBaseData data)
    {
        spellData = data;
        SpellBaseData = data;
    }
    #region �������ڷ���

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
            Debug.LogWarning("caster��Ҫʵ��IAttacker�ӿ�");
            enemyLayerMask = LayerMask.GetMask("Enemy");
        }
        if (spellData.isCastFromHand && caster is Player player)
        {
            transform.position = player.spellCastPos.position;
        }
    }


    protected virtual void Update()
    {
        // ����ʩ���ֲ�
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
    /// ����caster���Լ�Ҫʩ���ķ�λ�����һ����������ѡ������direction������������targetPosition����
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


        //Debug.Log($"����ʵ���ѳ�ʼ��: {GetType().Name}, ����: {castDirection}, ��ɫ��{spellData.spellColor}");
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

        // 1. �ȳ�ȥNature��Rock����ⷨ�������Ƿ��з�Ӧ
        bool hasFire = spellElements.Contains(MagicElement.Fire);
        bool hasWater = spellElements.Contains(MagicElement.Water);
        bool hasIce = spellElements.Contains(MagicElement.Ice);
        bool hasNature = spellElements.Contains(MagicElement.Nature);
        bool hasRock = spellElements.Contains(MagicElement.Rock);

        // ��������ķ�Ӧ���ˣ��������Ըĳ������߼�
        // ��+��
        if (hasFire && hasIce)
        {
            multiplier *= spellModel.FireIceReactionMultiplierInSpell;
            spellHasReacted = true;
        }
        // ��+ˮ
        else if (hasFire && hasWater)
        {
            multiplier *= spellModel.FireWaterReactionMultiplierInSpell;
            spellHasReacted = true;
        }
        // ��+ˮ
        else if (hasIce && hasWater)
        {
            // ���Է��Ҽ���Buff
            multiplier *= spellModel.IceWaterReactionMultiplierInSpell;
            spellHasReacted = true;
            target.BuffSystem.AddBuff<SlowDown>(caster.name, 1);
        }
        // ��������û��Ӧ�����Ŀ�����ϵ�Ԫ���Ƿ�ɷ�Ӧ���ⲿ���߼�������Ҫ�Ż�
        // Ԥ��ͨ��BuffTag����ʵ����ط�Ӧ�߼����˴����ٿ���
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
                        //Debug.Log("�Ƴ�Ŀ���Buff");
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
                        //Debug.Log("�Ƴ�Ŀ���Buff");
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
                    // �˺����䣬���Է��Ҽ���Buff
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
            else if // ��Ȼϵ��Ӧ
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

        // ���û�з�Ӧ����Ԫ��Buff
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
            Debug.Log($"���� {spellData.name} ��Ŀ�귢����Ԫ�ط�Ӧ�����˱���Ϊ {multiplier}");
        }
        return multiplier;
    }

    /// <summary>
    /// Ĭ��Χ��casterλ����ת
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
            // ������ң����������Ŀ�귽����
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
            // Channel���ͷ����Զ�����
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
    /// Channel����������������λ�ñ仯������Ŀ�귽λ
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
        Debug.Log($"�����˺�ǿ�ȣ�SpellAttack: {spellAttackDamage}");

        if (target is IBuffableEntity buffableTarget)
        {
            float reactionMultiplier = ProcessStatusDamageLogic(buffableTarget, castElements);
            Debug.Log($"��Ӧ���� MultiPlier: {reactionMultiplier}");
            spellAttackDamage *= reactionMultiplier;
        }

        Debug.Log($"�����˺�= {spellAttackDamage}");
        target.TakeDamage(spellAttackDamage);

        //Debug.Log($"�� {target.name} ��� {finalDamage} ���˺�");
    }

    /// <summary>
    /// �����߼��ǣ������˺�=�����˺�+�����߹�������������Ҫ���Ϸ�Ӧ�˺���
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
    /// ��ȡ��Χ�ڵ���ЧĿ��
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

    // ����Ƕȣ�����y������
    protected virtual bool IsInAngleRange(Vector3 direction, Vector3 toTarget, float maxAngle)
    {
        float angle = Vector3.Angle(direction, toTarget);
        return angle <= maxAngle / 2f;
    }

    // ����ˮƽ�Ƕȣ�����y��������Spray
    protected virtual bool IsInHorizontalAngleRange(Vector3 direction, Vector3 toTarget, float maxAngle)
    {
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;
        Vector3 horizontalToTarget = new Vector3(toTarget.x, 0, toTarget.z).normalized;

        float angle = Vector3.Angle(horizontalDirection, horizontalToTarget);
        return angle <= maxAngle / 2f;
    }

    // ����߶��ܷ��
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

    // ���ڿ����ö�����Ż�
    public void DestroySelf()
    {
        //ObjectPool.Recycle(gameObject);
        Destroy(gameObject);
    }

    #endregion
}
