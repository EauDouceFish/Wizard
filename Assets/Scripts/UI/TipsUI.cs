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
        "�ܺã�ͨ����ø���Ԫ�أ�����ϳ�ǿ��ķ���",
        "̽���������򣬻��ܵ��ˣ������Ի��ף����������",
        "С��ͼ������������򡣶���̽���������򣬵��С��ͼĿ��������ʦ�Ϳ���ʩչ����͹�ȥ��",
        "�ҵ�ͨ������Boss��·���������Ի��ʤ����ף�����~"
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
