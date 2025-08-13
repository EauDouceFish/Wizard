using QFramework;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UIPanelBase : MonoBehaviour, IController
{
    /// <summary>
    /// Start时隐藏
    /// </summary>
    [SerializeField] protected bool hideOnStart = false;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        if (hideOnStart)
        {
            gameObject.SetActive(false);
        }
    }

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