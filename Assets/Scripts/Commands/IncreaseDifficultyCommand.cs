using QFramework;

/// <summary>
/// 减小游戏难度命令
/// </summary>
public class IncreaseDifficultyCommand : AbstractCommand
{
    private readonly int levelToIncrease;

    public IncreaseDifficultyCommand(int level)
    {
        levelToIncrease = level;
    }

    protected override void OnExecute()
    {
        GameCoreModel gameModel = this.GetModel<GameCoreModel>();
        int newLevel = gameModel.DifficultyLevel + levelToIncrease;
        gameModel.SetDifficulty(newLevel);
    }
}
