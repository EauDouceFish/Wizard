using QFramework;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UIPanelBase : MonoBehaviour, IController
{
    /// <summary>
    /// 大部分UI在Awake时隐藏
    /// </summary>
    [SerializeField] protected bool hideOnAwake = true;

    protected virtual void Awake()
    {
        if (hideOnAwake)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 面板初始化
    /// </summary>
    public virtual void OnPanelInit() { }

    /// <summary>
    /// 面板显示时调用
    /// </summary>
    public virtual void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 面板隐藏时调用
    /// </summary>
    public virtual void HidePanel()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 面板销毁时调用
    /// </summary>
    public virtual void OnPanelDestroy() { }

    #region Architecture

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }

    #endregion
}