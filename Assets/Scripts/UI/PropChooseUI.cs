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

    // 当前Panel可选的所有道具
    private List<BlessItemUI> activeBlessItems = new List<BlessItemUI>();

    #region 生命周期
    protected override void Awake()
    {
        storage = this.GetUtility<Storage>();
        propModel = this.GetModel<PropModel>();
        attributeSystem = this.GetSystem<AttributeSystem>();
        this.RegisterEvent<GetHolyBlessingEvent>(OnGetHolyBlessingEvent).UnRegisterWhenCurrentSceneUnloaded();
        this.RegisterEvent<BlessPropSelectedEvent>(OnBlessPropSelectedEvent).UnRegisterWhenCurrentSceneUnloaded();
    }

    #endregion

    #region 主要处理

    // 初始化 稀有度-道具 对应字典

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

    #region UI交互

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

