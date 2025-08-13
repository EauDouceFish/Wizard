using PlayerSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributePanel : UIPanelBase
{
    [SerializeField] AttributeItem itemAttack;
    [SerializeField] AttributeItem itemHealth;

    private Player player;

    protected override void Awake()
    {
        base.Awake();
        this.RegisterEvent<PlayerCreatedEvent>(OnPlayerCreated);
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnPlayerCreated(PlayerCreatedEvent playerCreatedEvent)
    {
        Debug.Log("AttributePanel: 玩家已创建，UI绑定数值信息");
        BindPlayer(playerCreatedEvent.player);
    }

    private void BindPlayer(Player player)
    {
        this.player = player;
        //血量变化
        player.OnCurrentHealthChange += OnHealthChanged;
        player.OnMaxHealthChange += OnHealthChanged;
        //攻击力变化
        player.OnCurrentAttackChange += OnAttackChanged;
        UpdateUI();
    }

    private void OnHealthChanged(float change)
    {
        UpdateHealthDisplay();
    }
    private void OnAttackChanged(float change)
    {
        UpdateAttackDisplay();
    }


    private void UpdateUI()
    {
        UpdateAttackDisplay();
        UpdateHealthDisplay();
    }

    private void UpdateAttackDisplay()
    {
        int attack = Mathf.FloorToInt(player.CurrentAttack);
        itemAttack.SetAttack(attack);
    }

    private void UpdateHealthDisplay()
    {
        itemHealth.SetHealth(player.CurrentHealth, player.MaxHealth);
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnCurrentHealthChange -= OnHealthChanged;
            player.OnMaxHealthChange -= OnHealthChanged;
            player.OnCurrentAttackChange -= OnAttackChanged;
        }
    }
}