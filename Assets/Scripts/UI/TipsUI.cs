using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

public class TipsUI : UIPanelBase
{
    [SerializeField] TipsUIPanel tipsUIPanel;

    [SerializeField] float tipDisplayDuration = 10f;

    private int broadingIndex = 0;
    private bool tutorialStarted = false;

    private readonly string[] tips = {
        "很好！通过获得更多元素，能组合出强大的法术",
        "探索更多区域，击败敌人，还可以获得祝福提升能力",
        "小地图标记了所有区域。对于探索过的区域，点击小地图目标区域，巫师就可以施展意念传送过去！",
        "找到通向最终Boss的路，击败他以获得胜利。祝你好运~"
    };


    protected override void Awake()
    {
        this.RegisterEvent<OnElementGetEvent>(OnElementGetEventTip);
    }

    private void Update()
    {
        if (!tutorialStarted) return;
        if (Input.GetMouseButtonDown(0))
        {
            broadingIndex++;
            if (broadingIndex >= tips.Length)
            {
                tipsUIPanel.HidePanel();
                return;
            }
            tipsUIPanel.SetTipText(tips[broadingIndex]);
        }
    }

    private void OnElementGetEventTip(OnElementGetEvent evt)
    {
        StartTutorial();
        tutorialStarted = true;
    }

    public void StartTutorial()
    {
        StartCoroutine(ShowTipsSequence());
    }

    private IEnumerator ShowTipsSequence()
    {
        tipsUIPanel.ShowPanel();
        broadingIndex = 0;

        while (broadingIndex < tips.Length)
        {
            tipsUIPanel.SetTipText(tips[broadingIndex]);
            yield return new WaitForSeconds(tipDisplayDuration);
            broadingIndex++;
        }
        this.UnRegisterEvent<OnElementGetEvent>(OnElementGetEventTip);
        tipsUIPanel.HidePanel();
    }
}
