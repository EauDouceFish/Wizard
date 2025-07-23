using System;
public class BuffBase<T>
{
    public BuffData BuffData { get; private set; } = null;
    
    /// <summary>
    /// ��buff�ĳ����ߣ�������ʾ��buff������˭����
    /// </summary>
    public T Owner { get; private set; } = default;
    
    /// <summary>
    /// ��Buff���ṩ��,���ַ�������ʾ��
    /// </summary>
    public string Provider { get; private set; } = string.Empty;
    
    /// <summary>
    /// ����ʱ������
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
            throw new Exception("buff������г�ʼ��֮�����ʹ��");
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
    /// buff�ȼ�����������
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
            //ˢ�³���ʱ��
            if (BuffData.autoRefresh && change >= 0)
            {
                ResidualDuration = BuffData.maxDuration;
            }
        }
    }

    /// <summary>
    /// Buff��ʣ��ʱ�䡣
    /// ���ܳ���������ʱ�䣬����С��0��
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

    #region �鷽��

    /// <summary>
    /// ���buff�Ľ���
    /// </summary>
    /// <returns></returns>
    public virtual string GetDescription()
    {
        return BuffData.description;
    }
    /// <summary>
    /// ����buff����Ӻ󴥷�
    /// </summary>
    public virtual void AfterBeAdded() { }
    /// <summary>
    /// ����buff���Ƴ�ʱ����
    /// </summary>
    public virtual void AfterBeRemoved() { }
    /// <summary>
    /// ��Buff�ȼ��ı�ʱ����
    /// </summary>
    protected virtual void OnCurrentLevelChange(int change) { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }
    #endregion

    public BuffBase() { }
}
