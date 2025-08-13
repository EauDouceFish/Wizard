using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class BlessItemUI : MonoBehaviour, IController, ICanSendEvent
{
    private Button button;

    [SerializeField] private Image blessIcon;
    [SerializeField] private TextMeshProUGUI blessName;
    [SerializeField] private TextMeshProUGUI blessDesc;

    private AbstractBlessingProp assignedProp;
    private AttributeSystem attributeSystem;

    // 可以加个Color字段  

    private void Awake()
    {
        attributeSystem = this.GetSystem<AttributeSystem>();
        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }

        button.onClick.AddListener(() =>
        {
            this.SendEvent<BlessPropSelectedEvent>(new BlessPropSelectedEvent
            {
                SelectedProp = assignedProp
            });
        });
    }

    public void SetupBlessItemUI(AbstractBlessingProp prop)
    {
        PropData info = prop.PropData;
        blessIcon.sprite = info.icon;
        blessName.text = info.propName;
        blessDesc.text = info.propDesc;
        assignedProp = prop;
    }

    public void SetBlessIcon(Sprite icon)
    {
        blessIcon.sprite = icon;
    }

    public void SetBlessName(string blessName)
    {
        this.blessName.text = blessName;
    }

    public void SetBlessDesc(string blessDesc)
    {
        this.blessDesc.text = blessDesc;
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}