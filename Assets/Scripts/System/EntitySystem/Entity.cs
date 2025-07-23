using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IController, ICanSendEvent
{
    /// <summary>
    /// 最大生命值改变时触发
    /// </summary>
    public event Action<float> OnMaxHealthChange;
    /// <summary>
    /// 当前生命值改变时触发
    /// </summary>
    public event Action<float> OnCurrentHealthChange;

    private float m_MaxHealth = 50;
    private float m_CurrentHealth = 50;

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
            if (m_CurrentHealth == 0)
            {
                Dead();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
            return;

        CurrentHealth -= damage;
    }


    private void Dead()
    {
        gameObject.SetActive(false);
    }


    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
