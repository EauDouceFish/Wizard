using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// Buff����ʵ�� - ר�Ŵ�������/����Ч��
/// </summary>
public class BuffSpellInstance : BasicSpellInstance
{
    protected override void ExecuteSpell()
    {
        // Buff������һ���Ƕ�ʩ�����Լ���Ч
        if (caster == null)
        {
            Debug.LogWarning("BuffSpell: ʩ����Ϊ��");
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
    /// Ӧ������Ч��������ȻԪ�أ�
    /// </summary>
    private void ApplyHealingEffect()
    {
        // ����Ԫ����������������
        int elementCount = castElements?.Count ?? 1;
        float baseHealAmount = 15f;
        float finalHealAmount = baseHealAmount * elementCount;

        // ִ������
        HealEntity(caster, (int)finalHealAmount);

        Debug.Log($"��Ȼ����: {finalHealAmount} HP (����:{baseHealAmount} x Ԫ������:{elementCount})");
    }

    private void ApplySelfDamage()
    {
        int elementCount = castElements?.Count ?? 1;
        float baseDamage = CalculateElementSelfDamage();
        float finalDamage = baseDamage * elementCount;
        caster.TakeDamage((int)finalDamage);

        Debug.Log($"ħ������: {finalDamage} HP (����:{baseDamage} x Ԫ������:{elementCount})");
        Debug.Log($"�˺�Ԫ��: {string.Join(", ", castElements)}");
    }

    /// <summary>
    /// ���㲻ͬԪ�ص���������˺�
    /// </summary>
    private float CalculateElementSelfDamage()
    {
        float totalDamage = 0f;

        if (castElements == null || castElements.Count == 0)
            return 8f; // Ĭ�������˺�

        // ����Ԫ�����ͼ�������˺�
        foreach (var element in castElements)
        {
            switch (element)
            {
                case MagicElement.Fire:
                    totalDamage += 10f; // ��Ԫ������ϸߣ����ˣ�
                    break;
                case MagicElement.Ice:
                    totalDamage += 6f;  // ��Ԫ�������еȣ����ˣ�
                    break;
                case MagicElement.Rock:
                    totalDamage += 12f; // ��ʯԪ��������ߣ��ش���
                    break;
                case MagicElement.Water:
                    totalDamage += 4f;  // ˮԪ������ϵͣ���ˮ�У�
                    break;
                case MagicElement.Nature:
                    // ��ȻԪ�ز������������
                    break;
                default:
                    totalDamage += 8f;  // δ֪Ԫ��Ĭ���˺�
                    break;
            }
        }
        return totalDamage > 0 ? totalDamage / castElements.Count : 0f;
    }

    /// <summary>
    /// ����ʵ�壨��չ��������ΪEntityȱ��Heal������
    /// </summary>
    private void HealEntity(Entity target, int healAmount)
    {
        if (target == null || healAmount <= 0) return;

        // �������ƺ������ֵ�����������ֵ��
        float newHealth = Mathf.Min(target.CurrentHealth + healAmount, target.MaxHealth);
        target.CurrentHealth = newHealth;

        Debug.Log($"{target.name} �ָ��� {healAmount} ����ֵ����ǰ: {target.CurrentHealth}/{target.MaxHealth}");
    }

    /// <summary>
    /// ����Ƿ�Ϊ����ȻԪ��
    /// </summary>
    private bool IsAllNatureElements()
    {
        if (castElements == null || castElements.Count == 0)
            return false;

        // �������Ԫ���Ƿ�����ȻԪ��
        foreach (var element in castElements)
        {
            if (element != MagicElement.Nature)
                return false;
        }

        return true;
    }

    /// <summary>
    /// ��ȡԪ����ϵ������������ã�
    /// </summary>
    private string GetElementDescription()
    {
        if (castElements == null || castElements.Count == 0)
            return "��Ԫ��";

        return string.Join(" + ", castElements);
    }
}