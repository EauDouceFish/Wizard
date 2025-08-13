using PlayerSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有祝福道具的基类，获取后可以根据具体实现提升玩家属性
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

    // 懒加载获取Player实例
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
    /// 具体执行，给玩家上Buff
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
    /// 如果有需要，可以具体实现return BuffDataManager.GetBuffData<T>();
    /// </summary>
    public virtual BuffData GetBuffData()
    {
        Debug.LogWarning("请在具体实现中重写GetBuffData方法");
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
