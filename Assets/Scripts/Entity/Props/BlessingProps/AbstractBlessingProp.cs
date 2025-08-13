using PlayerSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ף�����ߵĻ��࣬��ȡ����Ը��ݾ���ʵ�������������
/// </summary>
public abstract class AbstractBlessingProp : MonoBehaviour, IController
{
    [SerializeField] PropData propInfo;

    protected PlayerModel playerModel;
    protected PropModel propModel => this.GetModel<PropModel>();
    private Player player;
    protected Player Player
    {
        get
        {
            if (player == null)
            {
                InitializePlayer();
            }
            return player;
        }
    }

    // �����ػ�ȡPlayerʵ��
    private void InitializePlayer()
    {
        if (playerModel == null)
        {
            playerModel = this.GetModel<PlayerModel>();
        }
        if (player == null)
        {
            player = playerModel.GetPlayer();
        }
    }
    /// <summary>
    /// ����ִ�У��������Buff
    /// </summary>
    public virtual void Execute()
    {
        if (propModel.OwningPropDict.TryGetValue(this, out int currentCount))
        {
            propModel.OwningPropDict[this] = currentCount + 1;
        }
        else
        {
            propModel.OwningPropDict[this] = 1;
        }
    }

    /// <summary>
    /// �������Ҫ�����Ծ���ʵ��return BuffDataManager.GetBuffData<T>();
    /// </summary>
    public virtual BuffData GetBuffData()
    {
        Debug.LogWarning("���ھ���ʵ������дGetBuffData����");
        return null;
    }

    public PropData PropData => propInfo;

    public string GetPropDesc()
    {
        return propInfo.propDesc;
    }

    public string GetPropName()
    {
        return propInfo.propName;
    }

    public int GetPropRarity()
    {
        return propInfo.rarity;
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
