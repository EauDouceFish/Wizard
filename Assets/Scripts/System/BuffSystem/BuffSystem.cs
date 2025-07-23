using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;
public class BuffSystem<Owner>
{
    #region ���캯��
    public BuffSystem(Owner owner)
    {
        m_Owner = owner;
    }
    #endregion

    #region �¼�
    public event Action<BuffBase<Owner>> OnAddBuff;
    public event Action<BuffBase<Owner>> OnRemoveBuff;
    #endregion

    private readonly Owner m_Owner;
    private readonly List<BuffBase<Owner>> m_Buffs = new List<BuffBase<Owner>>(10);
    private readonly List<string> m_BuffsPoolName = new List<string>(10);

    #region �ⲿ���÷���

    /// <summary>
    /// ���һ��buff
    /// </summary>
    /// <typeparam name="BuffType">buff����</typeparam>
    /// <param name="provider">buff�ṩ��</param>
    /// <param name="heap">��Ӷ��ٲ�?</param>
    public void AddBuff<BuffType>(string provider, int heap = 1) where BuffType : BuffBase<Owner>, new()
    {
        //�������û���κ�buff��ֱ�ӹ�һ����buff����
        if (m_Buffs.Count == 0)
        {
            AddNewBuff<BuffType>(provider, heap);
            return;
        }
        //��������������û���Ѵ��ڵ�ͬ��buff��Ȼ��������
        List<BuffType> buffs = new List<BuffType>(5);
        foreach (BuffBase<Owner> item in m_Buffs)
        {
            if (item is BuffType)
            {
                buffs.Add(item as BuffType);
            }
        }
        //���������ͬ��buff��ֱ�ӹ�һ����buff���ɣ�������г�ͻ����
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
                    //�ȼ����û��ͬһ��Դ�ģ��еĻ��������ӣ�û�Ļ����һ����buff
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

                    //�ȼ����û��ͬһ��Դ�ģ�
                    //û�Ļ��Ƴ���buff�������buff
                    //�еĻ��������ӣ�
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
    /// �Ƴ�ָ��buff
    /// �����buff�������򷵻�false
    /// </summary>
    /// <param name="buff">Ҫ�Ƴ���buff</param>
    /// <returns></returns>
    public bool RemoveBuff(BuffBase<Owner> buff)
    {
        //���ҵ�����
        int index = IndexOf(buff);
        if (index < 0)
        {
            return false;
        }
        //Ȼ���Ƴ�����
        RemoveAt(index);
        return true;
    }

    /// <summary>
    /// �������Ƴ�һ��buff
    /// �����buff�������򷵻�false
    /// </summary>
    /// <typeparam name="BuffType">Ҫ�Ƴ���buff������</typeparam>
    /// <param name="provider">Ҫ�Ƴ���buff���ṩ��</param>
    /// <returns></returns>
    public bool RemoveBuff<BuffType>(string provider) where BuffType : BuffBase<Owner>, new()
    {
        //�ҵ�����������buff
        BuffType result = FindBuff<BuffType>(provider);
        if (result == null)
        {
            return false;
        }
        //��ȡ��������
        int index = IndexOf(result);
        //�Ƴ���
        RemoveAt(index);
        return true;

    }
    /// <summary>
    /// ����һ��buff�����û�ҵ�����null
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
    /// ���ص�ǰӵ�е�����buff��
    /// </summary>
    /// <returns></returns>
    public List<BuffBase<Owner>> FindAllBuff()
    {
        return m_Buffs;
    }
    #endregion


    #region private����
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
        //����buff
        string poolName = typeof(Buff).FullName;
        Buff buff = ObjectPool.Get<Buff>(poolName);
        buff.Init(BuffDataManager.GetBuffData<Buff>());
        //��ʼ������
        buff.SetOwner(m_Owner, provider);
        buff.DurationScale = 1;
        buff.CurrentLevel = heap;
        buff.ResidualDuration = buff.BuffData.maxDuration;

        //���buff
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
            //ִ��fixed update
            item.FixedUpdate();
            //�������������buff����û�б������Ҫ��ʱ����д���
            if (!item.BuffData.isPermanent && item.TimeFreeze)
            {
                //���ͳ���ʱ��
                if (item.DurationScale <= 0)
                {
                    item.ResidualDuration = 0;
                }
                else
                {
                    item.ResidualDuration -= Time.fixedDeltaTime * (1 / item.DurationScale);
                }
                //�������ʱ�����0�����Ͷѵ�����
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
            //����ѵ�����Ϊ0�����Ƴ���buff��
            if (item.CurrentLevel == 0)
            {
                RemoveAt(i);
            }
        }
    }
    #endregion
}
