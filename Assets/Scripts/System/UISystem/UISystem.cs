using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

/// <summary>
/// 按照类型管理UI的系统，每种类型UIPanel只能有一个实例
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
    /// 打开UIPanel并推入栈（放在栈顶）
    /// </summary>
    public void OpenPanel(UIPanelBase panelBase)
    {
        UIPanelBase panel = panelBase;
        if (panel != null)
        {
            // 隐藏当前栈顶面板（如果有）  
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
    /// 关闭当前面板
    /// </summary>
    public void CloseCurrentPanel()
    {
        if (m_UIStack.Count > 0)
        {
            var panel = m_UIStack.Pop();
            panel.HidePanel();
            panel.gameObject.SetActive(false);

            // 显示下一个面板（如果有）
            if (m_UIStack.Count > 0)
            {
                var nextPanel = m_UIStack.Peek();
                nextPanel.ShowPanel();
            }
        }
    }

    /// <summary>
    /// 获取正在展示的Panel（没有返回null）
    /// </summary>
    public UIPanelBase GetCurrentPanel()
    {
        return m_UIStack.Count > 0 ? m_UIStack.Peek() : null;
    }

    /// <summary>
    /// 隐藏所有面板
    /// </summary>
    public void ClearAllPanels()
    {
        while (m_UIStack.Count > 0)
        {
            CloseCurrentPanel();
        }
    }
}