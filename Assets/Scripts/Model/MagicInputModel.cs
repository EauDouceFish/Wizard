using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class MagicInputModel : AbstractModel
{
    public const int MAX_INPUT_ELEMENTS = 5;
    // ����Ԫ�ؼ���
    private BindableProperty<HashSet<MagicElement>> availableElements = new BindableProperty<HashSet<MagicElement>>(new HashSet<MagicElement>());
    // ��ǰ�����Ԫ������
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
    /// �������Ԫ��
    /// </summary>
    public bool AddInputElement(MagicElement element)
    {
        // ���Ԫ���Ƿ����
        if (!availableElements.Value.Contains(element))
        {
            return false;
        }

        if (inputElements.Value.Count >= MAX_INPUT_ELEMENTS)
        {
            Debug.Log($"�Ѵﵽ�������Ԫ���������� ({MAX_INPUT_ELEMENTS})");
            return false;
        }

        var newList = new List<MagicElement>(inputElements.Value) { element };
        inputElements.Value = newList;
        return true;
    }

    /// <summary>
    /// ���������Ԫ��
    /// </summary>
    /// <param name="elements"></param>
    public void SetInputElements(List<MagicElement> elements)
    {
        inputElements.Value = elements;
    }

    /// <summary>
    /// �������Ԫ��
    /// </summary>
    public void ClearInputElements()
    {
        inputElements.Value = new List<MagicElement>();
    }

    /// <summary>
    /// ��ӿ���Ԫ��
    /// </summary>
    public void AddAvailableElement(MagicElement element)
    {
        var newSet = new HashSet<MagicElement>(availableElements.Value);
        newSet.Add(element);
        availableElements.Value = newSet;
    }

    /// <summary>
    /// �Ƴ�����Ԫ��
    /// </summary>
    public void RemoveAvailableElement(MagicElement element)
    {
        var newSet = new HashSet<MagicElement>(availableElements.Value);
        newSet.Remove(element);
        availableElements.Value = newSet;
    }

    public IArchitecture GetArchitecture() => GameCore.Interface;
}