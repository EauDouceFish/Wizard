using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

public class GameplayUI : UIPanelBase
{
    [Header("ħ������UI")]
    [SerializeField] private Transform inputElementsContainer;
    [SerializeField] private Transform availableElementsContainer;
    [SerializeField] private ElementIcon elementIconPrefab;

    [Header("Ԫ��ͼ��")]
    [SerializeField] private Sprite fireIcon;
    [SerializeField] private Sprite waterIcon;
    [SerializeField] private Sprite iceIcon;
    [SerializeField] private Sprite natureIcon;
    [SerializeField] private Sprite rockIcon;

    private MagicInputModel magicInputModel;

    // �洢��ǰ�����Ԫ��ͼ�ꡣ��Modelͬ������
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

    // һһ�ȶ�ͼ�꣬���������ͼ
    private void UpdateInputElementDisplay()
    {
        List<MagicElement> elements = magicInputModel.InputElements.Value;

        // �Ƴ����ࡢ���Ͳ�ƥ���ͼ�꣬�����ȱʧ��ͼ��
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

    // ���¿���Ԫ��ͼ��
    private void UpdateAvailableElementDisplay()
    {
        HashSet<MagicElement> availableElements = magicInputModel.AvailableElements.Value;

        foreach (MagicElement element in availableElements)
        {
            // ���ElementIcon�Ѿ�������availableElementIcons��������
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

        // ��Ӽ���ָ��
        if (keyboardTips != null)
        {
            keyboardTips.text = GetElementKeyHint(element);
        }

        availableElementIcons.Add(icon);
    }

    // ������ʾ
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

    // ��ȡԪ�ض�Ӧ��ͼ��
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

    #region �ⲿ����
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