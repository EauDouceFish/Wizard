using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using TMPro;

public class TipsUIPanel : UIPanelBase
{
    [SerializeField] TextMeshProUGUI tipText;
    public void SetTipText(string text)
    {
        if (tipText != null)
        {
            tipText.text = text;
        }
    }
}
