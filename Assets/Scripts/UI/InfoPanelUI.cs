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

    // 游戏难度变化
    private void OnDifficultyLevelChanged(int newDifficulty)
    {
        Debug.Log($"InfoPanelUI: 难度变化为 {newDifficulty}");

        if (difficultyInfo != null)
        {
            difficultyInfo.text = $"游戏难度： {newDifficulty}";
        }
        if (newDifficulty != 1) PlayTipAnimation();
    }

    // 当前难度下已征服的领域数量变化
    private void OnRegionCompletedCountChanged(int completedCount)
    {
        Debug.Log($"InfoPanelUI: 当前难度征服领域数变化为 {completedCount}");

        if (completeInfo != null)
        {
            completeInfo.text = $"当前难度已征服了 {completedCount} 个领域";
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