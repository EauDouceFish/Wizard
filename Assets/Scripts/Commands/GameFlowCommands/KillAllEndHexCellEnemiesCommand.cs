using QFramework;

public class KillAllEndHexCellEnemiesCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var gameEntityModel = this.GetModel<GameEntityModel>();
        var gameCoreModel = this.GetModel<GameCoreModel>();

        // ��Model�л�ȡ���л�Ծ���˲�����
        foreach (var hexCellEnemies in gameEntityModel.GetActiveEnemies(gameCoreModel.EndHexCell))
        {
            hexCellEnemies.GetComponent<Enemy>().TakeDamage(999999999);
        }

        Debugger.Log("�����˳������е��ˣ����999999999���˺�");
    }
}