using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// Buff法术实例 - 专门处理增益/减益效果
/// </summary>
public class BuffSpellInstance : BasicSpellInstance
{
    protected override void ExecuteSpell()
    {
        // Buff法术，一般是对施法者自己生效
        if (caster == null)
        {
            Debug.LogWarning("BuffSpell: 施法者为空");
            Destroy(gameObject);
            return;
        }

        if (IsAllNatureElements())
        {
            ApplyHealingEffect();
        }
        else
        {
            ApplySelfDamage();
        }
        PlaySpellEffect(caster.transform.position);

        Destroy(gameObject, 0.1f);
    }

    /// <summary>
    /// 应用治疗效果（纯自然元素）
    /// </summary>
    private void ApplyHealingEffect()
    {
        // 根据元素数量计算治疗量
        int elementCount = castElements?.Count ?? 1;
        float baseHealAmount = 15f;
        float finalHealAmount = baseHealAmount * elementCount;

        // 执行治疗
        HealEntity(caster, (int)finalHealAmount);

        Debug.Log($"自然治疗: {finalHealAmount} HP (基础:{baseHealAmount} x 元素数量:{elementCount})");
    }

    private void ApplySelfDamage()
    {
        int elementCount = castElements?.Count ?? 1;
        float baseDamage = CalculateElementSelfDamage();
        float finalDamage = baseDamage * elementCount;
        caster.TakeDamage((int)finalDamage);

        Debug.Log($"魔法反噬: {finalDamage} HP (基础:{baseDamage} x 元素数量:{elementCount})");
        Debug.Log($"伤害元素: {string.Join(", ", castElements)}");
    }

    /// <summary>
    /// 计算不同元素的自损基础伤害
    /// </summary>
    private float CalculateElementSelfDamage()
    {
        float totalDamage = 0f;

        if (castElements == null || castElements.Count == 0)
            return 8f; // 默认自损伤害

        // 根据元素类型计算基础伤害
        foreach (var element in castElements)
        {
            switch (element)
            {
                case MagicElement.Fire:
                    totalDamage += 10f; // 火元素自损较高（灼伤）
                    break;
                case MagicElement.Ice:
                    totalDamage += 6f;  // 冰元素自损中等（冻伤）
                    break;
                case MagicElement.Rock:
                    totalDamage += 12f; // 岩石元素自损最高（重创）
                    break;
                case MagicElement.Water:
                    totalDamage += 4f;  // 水元素自损较低（溺水感）
                    break;
                case MagicElement.Nature:
                    // 自然元素不参与自损计算
                    break;
                default:
                    totalDamage += 8f;  // 未知元素默认伤害
                    break;
            }
        }
        return totalDamage > 0 ? totalDamage / castElements.Count : 0f;
    }

    /// <summary>
    /// 治疗实体（扩展方法，因为Entity缺少Heal方法）
    /// </summary>
    private void HealEntity(Entity target, int healAmount)
    {
        if (target == null || healAmount <= 0) return;

        // 计算治疗后的生命值（不超过最大值）
        float newHealth = Mathf.Min(target.CurrentHealth + healAmount, target.MaxHealth);
        target.CurrentHealth = newHealth;

        Debug.Log($"{target.name} 恢复了 {healAmount} 生命值，当前: {target.CurrentHealth}/{target.MaxHealth}");
    }

    /// <summary>
    /// 检查是否为纯自然元素
    /// </summary>
    private bool IsAllNatureElements()
    {
        if (castElements == null || castElements.Count == 0)
            return false;

        // 检查所有元素是否都是自然元素
        foreach (var element in castElements)
        {
            if (element != MagicElement.Nature)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 获取元素组合的描述（调试用）
    /// </summary>
    private string GetElementDescription()
    {
        if (castElements == null || castElements.Count == 0)
            return "无元素";

        return string.Join(" + ", castElements);
    }
}