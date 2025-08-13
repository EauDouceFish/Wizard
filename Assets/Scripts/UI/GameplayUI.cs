using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

public class GameplayUI : UIPanelBase
{
    [Header("魔法输入UI")]
    [SerializeField] private Transform inputElementsContainer;
    [SerializeField] private Transform availableElementsContainer;
    [SerializeField] private ElementIcon elementIconPrefab;

    [Header("元素图标")]
    [SerializeField] private Sprite fireIcon;
    [SerializeField] private Sprite waterIcon;
    [SerializeField] private Sprite iceIcon;
    [SerializeField] private Sprite natureIcon;
    [SerializeField] private Sprite rockIcon;

    private MagicInputModel magicInputModel;

    // 存储当前输入的元素图标。由Model同步更新
    private List<ElementIcon> inputElementIcons = new List<ElementIcon>();
    private List<ElementIcon> availableElementIcons = new List<ElementIcon>();

    protected override void Awake()
    {
        base.Awake();
        magicInputModel = this.GetModel<MagicInputModel>();
    }

    protected override void Start()
    {
        BindModelEvents();
        UpdateInputElementDisplay();
        UpdateAvailableElementDisplay();
        base.Start();
    }

    private void BindModelEvents()
    {
        magicInputModel.InputElements.Register(OnInputElementsChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        magicInputModel.AvailableElements.Register(OnAvailableElementsChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnInputElementsChanged(List<MagicElement> newElements)
    {
        UpdateInputElementDisplay();
    }

    private void OnAvailableElementsChanged(HashSet<MagicElement> newElements)
    {
        UpdateAvailableElementDisplay();
    }

    // 一一比对图标，更新输入的图
    private void UpdateInputElementDisplay()
    {
        List<MagicElement> elements = magicInputModel.InputElements.Value;

        // 移除多余、类型不匹配的图标，再添加缺失的图标
        for (int i = inputElementIcons.Count - 1; i >= 0; i--)
        {
            if (i >= elements.Count || inputElementIcons[i] == null || inputElementIcons[i].magicElement != elements[i])
            {
                if (inputElementIcons[i] != null)
                {
                    Destroy(inputElementIcons[i].gameObject);
                }
                inputElementIcons.RemoveAt(i);
            }
        }

        for (int i = inputElementIcons.Count; i < elements.Count; i++)
        {
            CreateInputElementIcon(elements[i]);
        }
    }

    // 更新可用元素图标
    private void UpdateAvailableElementDisplay()
    {
        HashSet<MagicElement> availableElements = magicInputModel.AvailableElements.Value;

        foreach (MagicElement element in availableElements)
        {
            // 如果ElementIcon已经存在在availableElementIcons，则跳过
            bool exists = availableElementIcons.Exists(icon => icon.magicElement == element);
            if (exists) continue;
            CreateAvailableElementIcon(element);
        }
    }

    private void CreateInputElementIcon(MagicElement element)
    {
        ElementIcon elementIcon = Instantiate(elementIconPrefab, inputElementsContainer);
        elementIcon.magicElement = element;
        Image iconImage = elementIcon.GetComponent<Image>();
        iconImage.sprite = GetElementSprite(element);

        inputElementIcons.Add(elementIcon);
    }

    private void CreateAvailableElementIcon(MagicElement element)
    {
        ElementIcon icon = Instantiate(elementIconPrefab, availableElementsContainer);
        icon.magicElement = element;
        TextMeshProUGUI keyboardTips = icon.GetComponentInChildren<TextMeshProUGUI>();
        
        if (icon.TryGetComponent<Image>(out var iconImage))
        {
            iconImage.sprite = GetElementSprite(element);
            iconImage.color = new Color(1f, 1f, 1f, 0.5f);
        }

        // 添加键盘指引
        if (keyboardTips != null)
        {
            keyboardTips.text = GetElementKeyHint(element);
        }

        availableElementIcons.Add(icon);
    }

    // 输入提示
    private string GetElementKeyHint(MagicElement element)
    {
        return element switch
        {
            MagicElement.Fire => "Q",
            MagicElement.Water => "W",
            MagicElement.Ice => "A",
            MagicElement.Nature => "S",
            MagicElement.Rock => "D",
            _ => "?"
        };
    }

    // 获取元素对应的图标
    private Sprite GetElementSprite(MagicElement element)
    {
        return element switch
        {
            MagicElement.Fire => fireIcon,
            MagicElement.Water => waterIcon,
            MagicElement.Ice => iceIcon,
            MagicElement.Nature => natureIcon,
            MagicElement.Rock => rockIcon,
            _ => null
        };
    }

    #region 外部方法
    public void AddAvailableElement(MagicElement element)
    {
        magicInputModel.AddAvailableElement(element);
    }

    public void RemoveAvailableElement(MagicElement element)
    {
        magicInputModel.RemoveAvailableElement(element);
    }

    #endregion
}