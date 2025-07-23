using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;
public class BuffSystem<Owner>
{
    #region 构造函数
    public BuffSystem(Owner owner)
    {
        m_Owner = owner;
    }
    #endregion

    #region 事件
    public event Action<BuffBase<Owner>> OnAddBuff;
    public event Action<BuffBase<Owner>> OnRemoveBuff;
    #endregion

    private readonly Owner m_Owner;
    private readonly List<BuffBase<Owner>> m_Buffs = new List<BuffBase<Owner>>(10);
    private readonly List<string> m_BuffsPoolName = new List<string>(10);

    #region 外部可用方法

    /// <summary>
    /// 添加一个buff
    /// </summary>
    /// <typeparam name="BuffType">buff类型</typeparam>
    /// <param name="provider">buff提供者</param>
    /// <param name="heap">添加多少层?</param>
    public void AddBuff<BuffType>(string provider, int heap = 1) where BuffType : BuffBase<Owner>, new()
    {
        //如果身上没有任何buff则直接挂一个新buff即可
        if (m_Buffs.Count == 0)
        {
            AddNewBuff<BuffType>(provider, heap);
            return;
        }
        //遍历看看身上有没有已存在的同类buff，然后做处理
        List<BuffType> buffs = new List<BuffType>(5);
        foreach (BuffBase<Owner> item in m_Buffs)
        {
            if (item is BuffType)
            {
                buffs.Add(item as BuffType);
            }
        }
        //如果不存在同类buff，直接挂一个新buff即可，否则进行冲突处理
        if (buffs.Count == 0)
        {
            AddNewBuff<BuffType>(provider, heap);
        }
        else
        {
            BuffType sameProviderBuff = null;
            switch (buffs[0].BuffData.buffConflictResolution)
            {
                case BuffConflictResolution.Combine:
                    buffs[0].CurrentLevel += heap;
                    break;
                case BuffConflictResolution.Separate:
                    //先检测有没有同一来源的，有的话跟他叠加，没的话添加一个新buff
                    foreach (BuffType item in buffs)
                    {
                        if (item.Provider == provider)
                        {
                            sameProviderBuff = item;
                            break;
                        }
                    }
                    if (sameProviderBuff == null)
                    {
                        AddNewBuff<BuffType>(provider, heap);
                    }
                    else
                    {
                        sameProviderBuff.CurrentLevel += heap;
                    }
                    break;
                case BuffConflictResolution.Cover:

                    //先检测有没有同一来源的，
                    //没的话移除旧buff，添加新buff
                    //有的话跟他叠加，
                    foreach (BuffType item in buffs)
                    {
                        if (item.Provider == provider)
                        {
                            sameProviderBuff = item;
                            break;
                        }
                    }
                    if (sameProviderBuff == null)
                    {
                        RemoveBuff(sameProviderBuff);
                        AddNewBuff<BuffType>(provider, heap);
                    }
                    else
                    {
                        sameProviderBuff.CurrentLevel += heap;
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// 移除指定buff
    /// 如果此buff不存在则返回false
    /// </summary>
    /// <param name="buff">要移除的buff</param>
    /// <returns></returns>
    public bool RemoveBuff(BuffBase<Owner> buff)
    {
        //先找到索引
        int index = IndexOf(buff);
        if (index < 0)
        {
            return false;
        }
        //然后移除他！
        RemoveAt(index);
        return true;
    }

    /// <summary>
    /// 按条件移除一个buff
    /// 如果此buff不存在则返回false
    /// </summary>
    /// <typeparam name="BuffType">要移除的buff的类型</typeparam>
    /// <param name="provider">要移除的buff的提供者</param>
    /// <returns></returns>
    public bool RemoveBuff<BuffType>(string provider) where BuffType : BuffBase<Owner>, new()
    {
        //找到符号条件的buff
        BuffType result = FindBuff<BuffType>(provider);
        if (result == null)
        {
            return false;
        }
        //获取它的索引
        int index = IndexOf(result);
        //移除他
        RemoveAt(index);
        return true;

    }
    /// <summary>
    /// 查找一个buff，如果没找到返回null
    /// </summary>
    /// <typeparam name="BuffType"></typeparam>
    /// <param name="provider"></param>
    /// <returns></returns>
    public BuffType FindBuff<BuffType>(string provider) where BuffType : BuffBase<Owner>
    {
        BuffType result = null;
        foreach (BuffBase<Owner> item in m_Buffs)
        {
            if (item is BuffType)
            {
                if (item.Provider == provider)
                {
                    result = item as BuffType;
                }
            }
        }

        return result;
    }
    /// <summary>
    /// 返回当前拥有的所有buff。
    /// </summary>
    /// <returns></returns>
    public List<BuffBase<Owner>> FindAllBuff()
    {
        return m_Buffs;
    }
    #endregion


    #region private方法
    private int IndexOf(BuffBase<Owner> buff)
    {
        return m_Buffs.IndexOf(buff);
    }
    private BuffBase<Owner> RemoveAt(int index)
    {
        int buffsLastIndex = m_Buffs.Count - 1;
        int poolNameLastIndex = m_BuffsPoolName.Count - 1;

        BuffBase<Owner> result = m_Buffs[index];
        string poolName = m_BuffsPoolName[index];

        m_Buffs[index] = m_Buffs[buffsLastIndex];
        m_BuffsPoolName[index] = m_BuffsPoolName[poolNameLastIndex];

        m_Buffs.RemoveAt(buffsLastIndex);
        m_BuffsPoolName.RemoveAt(poolNameLastIndex);

        result.CurrentLevel = 0;
        result.AfterBeRemoved();
        OnRemoveBuff?.Invoke(result);

        ObjectPool.Release(result, poolName);

        return result;
    }
    private void AddNewBuff<Buff>(string provider, int heap = 1) where Buff : BuffBase<Owner>, new()
    {
        //创建buff
        string poolName = typeof(Buff).FullName;
        Buff buff = ObjectPool.Get<Buff>(poolName);
        buff.Init(BuffDataManager.GetBuffData<Buff>());
        //初始化设置
        buff.SetOwner(m_Owner, provider);
        buff.DurationScale = 1;
        buff.CurrentLevel = heap;
        buff.ResidualDuration = buff.BuffData.maxDuration;

        //添加buff
        m_Buffs.Add(buff);
        m_BuffsPoolName.Add(poolName);
        buff.AfterBeAdded();
        OnAddBuff?.Invoke(buff);
    }
    #endregion

    #region Update
    public void Update()
    {
        foreach (BuffBase<Owner> item in m_Buffs)
        {
            item.Update();
        }
    }
    public void FixedUpdate()
    {
        for (int i = m_Buffs.Count - 1; i >= 0; i--)
        {
            BuffBase<Owner> item = m_Buffs[i];
            //执行fixed update
            item.FixedUpdate();
            //如果不是永久性buff而且没有被冻结就要对时间进行处理
            if (!item.BuffData.isPermanent && item.TimeFreeze)
            {
                //降低持续时间
                if (item.DurationScale <= 0)
                {
                    item.ResidualDuration = 0;
                }
                else
                {
                    item.ResidualDuration -= Time.fixedDeltaTime * (1 / item.DurationScale);
                }
                //如果持续时间等于0，降低堆叠层数
                if (item.ResidualDuration == 0)
                {
                    if (item.BuffData.demotion > 0)
                    {
                        item.CurrentLevel -= item.BuffData.demotion;
                    }
                    else
                    {
                        item.CurrentLevel = 0;
                    }
                    item.ResidualDuration = item.BuffData.maxDuration;
                }
            }
            //如果堆叠层数为0，则移除此buff。
            if (item.CurrentLevel == 0)
            {
                RemoveAt(i);
            }
        }
    }
    #endregion
}
