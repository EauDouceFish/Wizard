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
        // ע��ս������¼�����
        this.RegisterEvent<OnBattleRegionClearedEvent>(OnBattleCompleted)
            .UnRegisterWhenGameObjectDestroyed(Owner.gameObject);
    }

    public override void AfterBeRemoved()
    {
        // Buff�Ƴ�ʱ�Զ�ȡ��ע��
        this.UnRegisterEvent<OnBattleRegionClearedEvent>(OnBattleCompleted);
    }

    private void OnBattleCompleted(OnBattleRegionClearedEvent evt)
    {
        // ���Owner�Ƿ���Player
        if (Owner is Player player)
        {
            float currentHealth = player.CurrentHealth;
            float maxHealth = player.MaxHealth;
            float healValue = healAmount * CurrentLevel; // �ȼ�Ӱ��ָ���

            player.CurrentHealth = Mathf.Min(currentHealth + healValue, maxHealth);

            Debug.Log($"��Ȼ�ָ�Buff���ָ� {healValue} ������ֵ");
        }
    }
    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
