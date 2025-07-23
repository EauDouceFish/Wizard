using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

/// <summary>
/// �������͹���UI��ϵͳ��ÿ������UIPanelֻ����һ��ʵ��
/// </summary>
public class UISystem : AbstractSystem
{
    private Stack<UIPanelBase> m_UIStack = new Stack<UIPanelBase>();
    private Storage storage;

    protected override void OnInit()
    {
        storage = this.GetUtility<Storage>();
    }

    /// <summary>
    /// ��UIPanel������ջ������ջ����
    /// </summary>
    public void OpenPanel(UIPanelBase panelBase)
    {
        UIPanelBase panel = panelBase;
        if (panel != null)
        {
            // ���ص�ǰջ����壨����У�  
            if (m_UIStack.Count > 0)
            {
                UIPanelBase currentPanel = m_UIStack.Peek();
                currentPanel.HidePanel();
            }

            panel.gameObject.SetActive(true);
            panel.ShowPanel();
            m_UIStack.Push(panel);
        }
    }

    /// <summary>
    /// �رյ�ǰ���
    /// </summary>
    public void CloseCurrentPanel()
    {
        if (m_UIStack.Count > 0)
        {
            var panel = m_UIStack.Pop();
            panel.HidePanel();
            panel.gameObject.SetActive(false);

            // ��ʾ��һ����壨����У�
            if (m_UIStack.Count > 0)
            {
                var nextPanel = m_UIStack.Peek();
                nextPanel.ShowPanel();
            }
        }
    }

    /// <summary>
    /// ��ȡ����չʾ��Panel��û�з���null��
    /// </summary>
    public UIPanelBase GetCurrentPanel()
    {
        return m_UIStack.Count > 0 ? m_UIStack.Peek() : null;
    }

    /// <summary>
    /// �����������
    /// </summary>
    public void ClearAllPanels()
    {
        while (m_UIStack.Count > 0)
        {
            CloseCurrentPanel();
        }
    }
}