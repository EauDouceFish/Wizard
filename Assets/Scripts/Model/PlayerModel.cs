using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using PlayerSystem;

/// <summary>
/// �洢Player����֮��
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
    /// ����ҳɹ�������ɵ��ã�����Playerʵ������
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
        CurrentHealth.Value = player.CurrentHealth; // Entity �����ȡ
        modelInited = true;
    }

    #region �����޸ķ���

    /// <summary>
    /// �޸Ĺ�����
    /// </summary>
    public void ModifyAttack(float amount)
    {
        CurrentAttack.Value += amount;
    }

    /// <summary>
    /// ���ù�����
    /// </summary>
    public void SetAttack(float value)
    {
        CurrentAttack.Value = value;
    }

    /// <summary>
    /// �����޸������޸�����ֵ
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
    /// �����������ֵ
    /// </summary>
    public void SetMaxHealth(float value)
    {
        MaxHealth.Value = value;
        // ȷ����ǰ����ֵ���������ֵ
        CurrentHealth.Value = Mathf.Min(CurrentHealth.Value, MaxHealth.Value);
    }

    #endregion

    #endregion
}