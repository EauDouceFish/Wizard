using PlayerSystem;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NatureRecovery : EntityBuffBase, ICanRegisterEvent
{
    private float healAmount = 15f;

    public override void AfterBeAdded()
    {
        // 注册战斗完成事件监听
        this.RegisterEvent<OnBattleRegionClearedEvent>(OnBattleCompleted)
            .UnRegisterWhenGameObjectDestroyed(Owner.gameObject);
    }

    public override void AfterBeRemoved()
    {
        // Buff移除时自动取消注册
        this.UnRegisterEvent<OnBattleRegionClearedEvent>(OnBattleCompleted);
    }

    private void OnBattleCompleted(OnBattleRegionClearedEvent evt)
    {
        // 检查Owner是否是Player
        if (Owner is Player player)
        {
            float currentHealth = player.CurrentHealth;
            float maxHealth = player.MaxHealth;
            float healValue = healAmount * CurrentLevel; // 等级影响恢复量

            player.CurrentHealth = Mathf.Min(currentHealth + healValue, maxHealth);

            Debug.Log($"自然恢复Buff，恢复 {healValue} 点生命值");
        }
    }
    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
