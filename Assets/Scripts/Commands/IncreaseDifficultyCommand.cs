using QFramework;

/// <summary>
/// ��С��Ϸ�Ѷ�����
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
