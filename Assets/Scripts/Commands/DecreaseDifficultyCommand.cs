using QFramework;

/// <summary>
/// 减小游戏难度命令
/// </summary>
public class DecreaseDifficultyCommand : AbstractCommand
{
    private readonly int levelToDecrease;

    public DecreaseDifficultyCommand(int level)
    {
        levelToDecrease = level;
    }

    protected override void OnExecute()
    {
        GameCoreModel model = this.GetModel<GameCoreModel>();
        int newLevel = model.DifficultyLevel - levelToDecrease;
        model.SetDifficulty(newLevel);
    }
}
