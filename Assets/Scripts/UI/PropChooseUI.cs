using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;
using UnityEngine.Rendering.Universal;

public class PropChooseUI : UIPanelBase
{
    Storage storage;
    AttributeSystem attributeSystem;
    PropModel propModel;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject container;
    [SerializeField] BlessItemUI blessItemPrefab;

    // ��ǰPanel��ѡ�����е���
    private List<BlessItemUI> activeBlessItems = new List<BlessItemUI>();

    #region ��������
    protected override void Awake()
    {
        storage = this.GetUtility<Storage>();
        propModel = this.GetModel<PropModel>();
        attributeSystem = this.GetSystem<AttributeSystem>();
        this.RegisterEvent<GetHolyBlessingEvent>(OnGetHolyBlessingEvent).UnRegisterWhenCurrentSceneUnloaded();
        this.RegisterEvent<BlessPropSelectedEvent>(OnBlessPropSelectedEvent).UnRegisterWhenCurrentSceneUnloaded();
    }

    #endregion

    #region ��Ҫ����

    // ��ʼ�� ϡ�ж�-���� ��Ӧ�ֵ�

    private void OnGetHolyBlessingEvent(GetHolyBlessingEvent evt)
    {
        var selectedProps = propModel.GetRandomBlessingPropsFiltered(1, 3);
        propModel.SetCurrentAvailableProps(selectedProps);

        ShowPropsSelection(selectedProps);
        panel.SetActive(true);
    }

    private void OnBlessPropSelectedEvent(BlessPropSelectedEvent evt)
    {
        panel.SetActive(false);
    }

    #endregion

    #region UI����

    private void ShowPropsSelection(List<AbstractBlessingProp> props)
    {
        ClearExistingBlessItems();

        foreach (AbstractBlessingProp prop in props)
        {
            CreateBlessItemUI(prop);
        }
    }


    private void ClearExistingBlessItems()
    {
        foreach (BlessItemUI item in activeBlessItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        activeBlessItems.Clear();
    }

    private void CreateBlessItemUI(AbstractBlessingProp prop)
    {
        BlessItemUI blessItem = Instantiate(blessItemPrefab, container.transform);
        blessItem.SetupBlessItemUI(prop);

        activeBlessItems.Add(blessItem);
    }
    #endregion
}

