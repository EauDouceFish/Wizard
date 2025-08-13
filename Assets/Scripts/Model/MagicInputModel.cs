using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class MagicInputModel : AbstractModel
{
    public const int MAX_INPUT_ELEMENTS = 5;
    // 可用元素集合
    private BindableProperty<HashSet<MagicElement>> availableElements = new BindableProperty<HashSet<MagicElement>>(new HashSet<MagicElement>());
    // 当前输入的元素序列
    private BindableProperty<List<MagicElement>> inputElements = new BindableProperty<List<MagicElement>>(new List<MagicElement>());

    public IBindableProperty<HashSet<MagicElement>> AvailableElements => availableElements;
    public IBindableProperty<List<MagicElement>> InputElements => inputElements;

    protected override void OnInit()
    {
        //SetAllElementsAvailable();
    }

    private void SetAllElementsAvailable()
    {
        var initialElements = new HashSet<MagicElement>
        {
            MagicElement.Fire,
            MagicElement.Water,
            MagicElement.Ice,
            MagicElement.Nature,
            MagicElement.Rock
        };
        availableElements.Value = initialElements;
    }

    /// <summary>
    /// 添加输入元素
    /// </summary>
    public bool AddInputElement(MagicElement element)
    {
        // 检查元素是否可用
        if (!availableElements.Value.Contains(element))
        {
            return false;
        }

        if (inputElements.Value.Count >= MAX_INPUT_ELEMENTS)
        {
            Debug.Log($"已达到最大输入元素数量限制 ({MAX_INPUT_ELEMENTS})");
            return false;
        }

        var newList = new List<MagicElement>(inputElements.Value) { element };
        inputElements.Value = newList;
        return true;
    }

    /// <summary>
    /// 设置输入的元素
    /// </summary>
    /// <param name="elements"></param>
    public void SetInputElements(List<MagicElement> elements)
    {
        inputElements.Value = elements;
    }

    /// <summary>
    /// 清空输入元素
    /// </summary>
    public void ClearInputElements()
    {
        inputElements.Value = new List<MagicElement>();
    }

    /// <summary>
    /// 添加可用元素
    /// </summary>
    public void AddAvailableElement(MagicElement element)
    {
        var newSet = new HashSet<MagicElement>(availableElements.Value);
        newSet.Add(element);
        availableElements.Value = newSet;
    }

    /// <summary>
    /// 移除可用元素
    /// </summary>
    public void RemoveAvailableElement(MagicElement element)
    {
        var newSet = new HashSet<MagicElement>(availableElements.Value);
        newSet.Remove(element);
        availableElements.Value = newSet;
    }

    public IArchitecture GetArchitecture() => GameCore.Interface;
}