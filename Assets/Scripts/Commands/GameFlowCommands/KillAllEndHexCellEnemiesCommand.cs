using QFramework;

public class KillAllEndHexCellEnemiesCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var gameEntityModel = this.GetModel<GameEntityModel>();
        var gameCoreModel = this.GetModel<GameCoreModel>();

        // 从Model中获取所有活跃敌人并销毁
        foreach (var hexCellEnemies in gameEntityModel.GetActiveEnemies(gameCoreModel.EndHexCell))
        {
            hexCellEnemies.GetComponent<Enemy>().TakeDamage(999999999);
        }

        Debugger.Log("清理了场上所有敌人！造成999999999点伤害");
    }
}