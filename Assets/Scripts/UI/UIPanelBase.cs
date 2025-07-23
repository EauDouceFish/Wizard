using QFramework;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UIPanelBase : MonoBehaviour, IController
{
    /// <summary>
    /// �󲿷�UI��Awakeʱ����
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
    /// ����ʼ��
    /// </summary>
    public virtual void OnPanelInit() { }

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