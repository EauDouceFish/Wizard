using System;
public class BuffBase<T>
{
    public BuffData BuffData { get; private set; } = null;
    
    /// <summary>
    /// 此buff的持有者，用来表示此buff作用在谁身上
    /// </summary>
    public T Owner { get; private set; } = default;
    
    /// <summary>
    /// 此Buff的提供者,用字符串来表示。
    /// </summary>
    public string Provider { get; private set; } = string.Empty;
    
    /// <summary>
    /// 持续时间缩放
    /// </summary>
    public float DurationScale { get; set; } = 1;

    private int m_CurrentLevel = 0;
    private float m_ResidualDuration = 0;
    private int m_TimeFreeze = 0;

    private bool m_Initialized = false;

    public void SetOwner(T owner, string provider)
    {
        if (!m_Initialized)
        {
            throw new Exception("buff必须进行初始化之后才能使用");
        }
        Owner = owner;
        Provider = provider;
    }

    public bool TimeFreeze
    {
        get { return m_TimeFreeze == 0; }
        set
        {
            if (value)
            {
                m_TimeFreeze += 1;
            }
            else
            {
                m_TimeFreeze -= 1;
            }
        }
    }
    
    /// <summary>
    /// buff等级（层数）。
    /// </summary>
    public int CurrentLevel
    {
        get
        {
            return m_CurrentLevel;
        }
        set
        {
            value = Math.Clamp(value, 0, BuffData.maxLevel);
            int change = value - m_CurrentLevel;
            OnCurrentLevelChange(change);
            m_CurrentLevel = value;
            //刷新持续时间
            if (BuffData.autoRefresh && change >= 0)
            {
                ResidualDuration = BuffData.maxDuration;
            }
        }
    }

    /// <summary>
    /// Buff的剩余时间。
    /// 不能超过最大持续时间，不能小于0。
    /// </summary>
    public float ResidualDuration
    {
        get { return m_ResidualDuration; }
        set { m_ResidualDuration = Math.Clamp(value, 0, BuffData.maxDuration); }
    }

    public void Init(BuffData buffData)
    {
        if (m_Initialized)
        {
            return;
        }
        BuffData = buffData;
        m_Initialized = true;
    }

    #region 虚方法

    /// <summary>
    /// 获得buff的介绍
    /// </summary>
    /// <returns></returns>
    public virtual string GetDescription()
    {
        return BuffData.description;
    }
    /// <summary>
    /// 当此buff被添加后触发
    /// </summary>
    public virtual void AfterBeAdded() { }
    /// <summary>
    /// 当此buff被移除时触发
    /// </summary>
    public virtual void AfterBeRemoved() { }
    /// <summary>
    /// 当Buff等级改变时触发
    /// </summary>
    protected virtual void OnCurrentLevelChange(int change) { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }
    #endregion

    public BuffBase() { }
}
