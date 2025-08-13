using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using PlayerSystem;

/// <summary>
/// 存储Player引用之用
/// </summary>
public class PlayerModel : AbstractModel, ICanRegisterEvent
{
    Player player;

    public BindableProperty<float> CurrentAttack { get; } = new BindableProperty<float>();
    public BindableProperty<float> MaxHealth { get; } = new BindableProperty<float>();
    public BindableProperty<float> CurrentHealth { get; } = new BindableProperty<float>();

    public HexCell currentHexCell;

    public bool modelInited = false;

    protected override void OnInit()
    {
    }

    /// <summary>
    /// 在玩家成功创建后可调用，返回Player实例引用
    /// </summary>
    public Player GetPlayer()
    {
        return player;
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
        InitializePlayerStats();
    }


    #region Architecture

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }

    private void InitializePlayerStats()
    {
        CurrentAttack.Value = player.playerData.playerBaseAttack;
        MaxHealth.Value = player.playerData.playerBaseHealth;
        CurrentHealth.Value = player.CurrentHealth; // Entity 基类获取
        modelInited = true;
    }

    #region 属性修改方法

    /// <summary>
    /// 修改攻击力
    /// </summary>
    public void ModifyAttack(float amount)
    {
        CurrentAttack.Value += amount;
    }

    /// <summary>
    /// 设置攻击力
    /// </summary>
    public void SetAttack(float value)
    {
        CurrentAttack.Value = value;
    }

    /// <summary>
    /// 按照修改量，修改生命值
    /// </summary>
    public void ModifyCurrentHealth(float amount)
    {
        CurrentHealth.Value = Mathf.Clamp(CurrentHealth.Value + amount, 0, MaxHealth.Value);
    }

    public void SetCurrentHealth(float value)
    {
        CurrentHealth.Value = Mathf.Clamp(value, 0, MaxHealth.Value);
    }

    /// <summary>
    /// 设置最大生命值
    /// </summary>
    public void SetMaxHealth(float value)
    {
        MaxHealth.Value = value;
        // 确保当前生命值不超过最大值
        CurrentHealth.Value = Mathf.Min(CurrentHealth.Value, MaxHealth.Value);
    }

    #endregion

    #endregion
}