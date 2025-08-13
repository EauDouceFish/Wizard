using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using TMPro;

public class InfoPanelUI : UIPanelBase
{
    [SerializeField] TextMeshProUGUI difficultyInfo;
    [SerializeField] TextMeshProUGUI completeInfo;
    [SerializeField] TextMeshProUGUI tipInfo;
    Animator tipInfoAnimator;

    private GameCoreModel gameCoreModel;

    protected override void Awake()
    {
        base.Awake();
        gameCoreModel = this.GetModel<GameCoreModel>();

        tipInfoAnimator = tipInfo.GetComponent<Animator>();
    }

    protected override void Start()
    {
        gameCoreModel.DifficultyLevel.RegisterWithInitValue(OnDifficultyLevelChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);

        gameCoreModel.RegionCompletedCountCurrentLevel.RegisterWithInitValue(OnRegionCompletedCountChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        base.Start();
    }

    // ��Ϸ�Ѷȱ仯
    private void OnDifficultyLevelChanged(int newDifficulty)
    {
        Debug.Log($"InfoPanelUI: �Ѷȱ仯Ϊ {newDifficulty}");

        if (difficultyInfo != null)
        {
            difficultyInfo.text = $"��Ϸ�Ѷȣ� {newDifficulty}";
        }
        if (newDifficulty != 1) PlayTipAnimation();
    }

    // ��ǰ�Ѷ��������������������仯
    private void OnRegionCompletedCountChanged(int completedCount)
    {
        Debug.Log($"InfoPanelUI: ��ǰ�Ѷ������������仯Ϊ {completedCount}");

        if (completeInfo != null)
        {
            completeInfo.text = $"��ǰ�Ѷ��������� {completedCount} ������";
        }
    }

    public void PlayTipAnimation()
    {
        if (tipInfoAnimator != null)
        {
            tipInfoAnimator.SetTrigger("PlayTipAnimation");
        }
    }
}