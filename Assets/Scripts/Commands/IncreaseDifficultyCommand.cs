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

        if (gameModel.isElementGetted == false)
        {
            gameModel.isElementGetted = true;
        }
        else
        {
            int newLevel = gameModel.DifficultyLevel.Value + levelToIncrease;
            gameModel.SetDifficulty(newLevel);
        }

    }
}
