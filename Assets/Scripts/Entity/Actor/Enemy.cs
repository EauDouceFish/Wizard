using PlayerSystem;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Enemy : AIActor, IHasHealthUI, IAttacker, IHasBuffSpeedMultiplier, IHasStatusUI, IBuffableEntity
{
    [SerializeField] public EnemyEntityData enemyData;

    protected Storage storage;
    protected HealthUI healthUI;
    protected StatusUI statusUI;
    private Actor attackTarget;

    private float runtimeAttack = 0;
    
    #region Attributes

    public bool IsInAttackRange => attackTarget != null && WithinAttackRange(attackTarget);
    public bool HasAttackTarget => attackTarget != null;

    public float CurrentAttack
    {
        get => runtimeAttack;
        set
        {
            runtimeAttack = value;
        }
    }

    public LayerMask AttackLayerMask { get => enemyData.enemyAttackLayerMask;}

    public Actor AttackTarget
    {
        get => attackTarget;
        set
        {
            attackTarget = value;
        }
    }

    public float BuffSpeedMultiplier
    {
        get => steeringBehaviours.SpeedBuffMultiplier;
        set
        {
            steeringBehaviours.SpeedBuffMultiplier = value;
        }
    }

    public BuffSystem<Entity> BuffSystem { get; set; }

    public Material VisualMaterial
    {
        get => GetComponentInChildren<Renderer>().material;
        set
        {
            if (GetComponentInChildren<Renderer>())
            {
                GetComponentInChildren<Renderer>().material = value;
            }
        }
    }

    #endregion

    #region 生命周期

    protected override void Awake()
    {
        base.Awake();
        storage = this.GetUtility<Storage>();
        BuffSystem = new BuffSystem<Entity>(this);
        InitEnemyData();
        InitHealthUI();
        InitStatusUI();
    }

    protected override void Start()
    {
        base.Start();
    }


    protected override void Update()
    {
        base.Update();
        BuffSystem.Update();
        UpdateAttackTarget();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        BuffSystem.FixedUpdate();
    }
    #endregion

    /// <summary>
    /// 需要在Awake初始化，否则会被外部先修改导致Init覆盖对方
    /// </summary>
    private void InitEnemyData()
    {
        runtimeAttack = enemyData.enemyBaseAttack;
        CurrentHealth = enemyData.enemyBaseHealth;
        MaxHealth = CurrentHealth;
        Debug.Log($"Enemy属性初始化: Health={CurrentHealth}, MaxHealth={MaxHealth}, Attack={runtimeAttack}");
    }

    public Actor GetNearestAttackTargetInView()
    {
        ActorTypeFilter filter = (actor) => actor is Player;
        List<Actor> targets = GetActorsInView(filter);

        if (targets.Count == 0) return null;

        targets.Sort((a, b) =>
        {
            float distA = Vector3.Distance(a.transform.position, transform.position);
            float distB = Vector3.Distance(b.transform.position, transform.position);
            return distA.CompareTo(distB);
        });

        return targets[0];
    }

    #region 视觉效果

    // UI
    public void BindHealthUI(HealthUI healthUI)
    {
        this.healthUI = healthUI;
        UpdateHealthUI(CurrentHealth, MaxHealth);
    }

    public void UpdateHealthUI(float current, float max)
    {
        if (healthUI != null)
        {
            healthUI.SetHealth(current, max);
        }
    }

    public void BindStatusUI(StatusUI statusUI)
    {
        this.statusUI = statusUI;
        BuffSystem.StatusUIOwner = this;
        if (statusUI)
        {
            UpdateStatusUI(BuffSystem.FindAllBuff());
        }
    }

    public void UpdateStatusUI(List<BuffBase<Entity>> buffs)
    {
        if (statusUI)
        {
            statusUI.SetBuffs(buffs);
        }
    }

    public void SetVisualColor(Color color)
    {
        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            MaterialPropertyBlock materialPropertyBlock = new();
            renderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor("_BaseColor", color);
            renderer.SetPropertyBlock(materialPropertyBlock);
        }
    }

    public void SetDissolveValue(float dissolveValue)
    {
        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            MaterialPropertyBlock materialPropertyBlock = new();
            renderer.GetPropertyBlock(materialPropertyBlock);

            if (renderer.material.HasProperty("_Dissolve"))
            {
                materialPropertyBlock.SetFloat("_Dissolve", dissolveValue);
                renderer.SetPropertyBlock(materialPropertyBlock);
            }
            else
            {
                Debug.LogWarning("对方目标材质不存在，No _Dissolve property.无法修改");
            }
        }
    }

    public void PlayDeathDissolve(float duration = 1f, Action onComplete = null)
    {
        StartCoroutine(DissolveCoroutine(duration, onComplete));
    }

    private IEnumerator DissolveCoroutine(float duration, Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float dissolveValue = elapsed / duration;
            SetDissolveValue(dissolveValue);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetDissolveValue(1f);
        onComplete?.Invoke();
    }

    #endregion

    public Animator GetAnimator()
    {
        if (animator == null)
        {
            Debug.LogError("找不到Actor动画机！还未赋值");
            return null;
        }
        return animator;
    }

    #region Reusable Methods

    protected override void Dead()
    {
        if (statusUI)
        {
            statusUI.gameObject.SetActive(false);
        }
        PlayDeathDissolve(0.5f, () => { Destroy(gameObject); });
    }

    #endregion

    #region 私有方法

    #region Visual


    protected virtual void InitHealthUI()
    {
        GameObject healthUIPrefab = storage.GetHealthUIPrefab();
        if (healthUIPrefab)
        {
            GameObject healthUIObject = Instantiate(healthUIPrefab, transform);
            healthUIObject.transform.localPosition = new Vector3(0, 2.0f, 0);
            healthUI = healthUIObject.GetComponent<HealthUI>();
            BindHealthUI(healthUI);
        }
    }

    protected virtual void InitStatusUI()
    {
        GameObject statusUIPrefab = storage.GetStatusUIPrefab();
        if (statusUIPrefab)
        {
            GameObject statusUIObject = Instantiate(statusUIPrefab, transform);
            statusUIObject.transform.localPosition = new Vector3(0, 3f, 0);
            statusUI = statusUIObject.GetComponent<StatusUI>();
            BindStatusUI(statusUI);
        }
    }
    #endregion
    private void UpdateAttackTarget()
    {
        if (attackTarget)
        {
            if (!WithinAttackRange(attackTarget))
            {
                attackTarget = null;
            }
        }
        if (attackTarget == null)
        {
            attackTarget = GetNearestAttackTargetInView();
        }
    }

    private bool WithinAttackRange(Actor actor)
    {
        if (actor == null) return false;
        return Vector3.Distance(actor.transform.position, transform.position) < attackRadius;
    }

    #endregion

}
