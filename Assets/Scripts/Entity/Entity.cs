using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IController, ICanSendEvent
{
    // 考虑改为BindbaleProperty
    /// <summary>  
    /// 最大生命值改变时触发  
    /// </summary>  
    public event Action<float> OnMaxHealthChange;
    /// <summary>  
    /// 当前生命值改变时触发  
    /// </summary>  
    public event Action<float> OnCurrentHealthChange;

    [SerializeField] private float m_MaxHealth = 1;
    [SerializeField] private float m_CurrentHealth = 1;

    public float MaxHealth
    {
        get { return m_MaxHealth; }
        set
        {
            float change = value - m_MaxHealth;
            m_MaxHealth = value;
            OnMaxHealthChange?.Invoke(change);
        }
    }

    public float CurrentHealth
    {
        get
        {
            return m_CurrentHealth;
        }
        set
        {
            float change = value - m_CurrentHealth;
            m_CurrentHealth = Mathf.Clamp(value, 0, m_MaxHealth);
            OnCurrentHealthChange?.Invoke(change);
            OnHealthChanged();
            if (m_CurrentHealth == 0)
            {
                Dead();
            }
        }
    }

    public virtual void OnHealthChanged()
    {
        if (this is IHasHealthUI hasHealthUI)
        {
            hasHealthUI.UpdateHealthUI(CurrentHealth, MaxHealth);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (damage <= 0) return;
        if (CurrentHealth <= 0) return;
        CurrentHealth -= damage;
    }

    protected virtual void Dead()
    {
        Destroy(gameObject, 0.5f);
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
