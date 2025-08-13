using QFramework;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UIPanelBase : MonoBehaviour, IController
{
    /// <summary>
    /// Startʱ����
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
    /// �����ʾʱ����
    /// </summary>
    public virtual void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// �������ʱ����
    /// </summary>
    public virtual void HidePanel()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �������ʱ����
    /// </summary>
    public virtual void OnPanelDestroy() { }

    #region Architecture

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }

    #endregion
}