using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCoreModel : AbstractModel
{
    public GameCoreController GameCoreController
    {
        get
        {
            if (gameCoreController == null)
            {
                gameCoreController = UnityEngine.Object.FindObjectOfType<GameCoreController>();

                if (gameCoreController == null)
                {
                    gameCoreController = new GameObject("GameCoreController").AddComponent<GameCoreController>();
                }
            }
            return gameCoreController;
        }
    }
    private GameCoreController gameCoreController;

    public BindableProperty<int> DifficultyLevel { get; private set; } = new BindableProperty<int>(1);
    public BindableProperty<int> RegionCompletedCountCurrentLevel { get; set; } = new BindableProperty<int>(0);
    public BindableProperty<bool> IsGamePaused { get; private set; } = new BindableProperty<bool>(false);


    public bool isBattling { get; set; } = false;

    public bool isElementGetted { get; set; } = false;

    public bool isGameEnded { get; set; } = false;

    public HexCell EndHexCell { get; set; } = null;

    protected override void OnInit()
    {
    }
    /// <summary>
    /// GameCoreModel内设置难度唯一方法，之后重置难度系数
    /// </summary>
    /// <param name="level"></param>
    public void SetDifficulty(int level)
    {
        if (level <= 0)
        {
            Debug.LogWarning("难度等级必须大于0！！");
        }
        DifficultyLevel.Value = Mathf.Max(1, level);
        RegionCompletedCountCurrentLevel.Value = 0;
    }

    public void SetGamePaused(bool isPaused)
    {
        IsGamePaused.Value = isPaused;
    }
}
