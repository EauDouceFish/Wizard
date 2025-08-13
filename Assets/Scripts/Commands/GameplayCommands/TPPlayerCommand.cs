using UnityEngine;
using QFramework;

public class TPPlayerCommand : AbstractCommand
{
    HexCell hexCell;
    private Pos2D coord;

    public TPPlayerCommand(HexCell cell)
    {
        this.hexCell = cell;
        this.coord = cell.coord;
    }

    protected override void OnExecute()
    {
        var playerModel = this.GetModel<PlayerModel>();
        var mapModel = this.GetModel<MapModel>();
        var gameCoreModel = this.GetModel<GameCoreModel>();

        if (gameCoreModel.isBattling)
        {
            Debug.Log("ս�����޷�����");
            return;
        }

        Vector3 teleportPosition = CalculateRandomTeleportPosition(hexCell);

        var player = playerModel.GetPlayer();
        player.transform.position = teleportPosition;

        Debug.Log($"��Ҵ��͵�����({coord.x}, {coord.y})");
    }

    /// <summary>
    /// ����HexCell����20%-40%��Χ�ڵ��������λ��
    /// </summary>
    private Vector3 CalculateRandomTeleportPosition(HexCell hexCell)
    {
        Vector3 centerPosition = hexCell.transform.position;

        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        float minRadius = hexCell.outerRadius * 0.2f;
        float maxRadius = hexCell.outerRadius * 0.4f;
        float randomRadius = Random.Range(minRadius, maxRadius);

        float offsetX = Mathf.Cos(randomAngle) * randomRadius;
        float offsetZ = Mathf.Sin(randomAngle) * randomRadius;

        Vector3 targetPosition = centerPosition + new Vector3(offsetX, 0, offsetZ);

        targetPosition.y = GOExtensions.GetGroundPosition(targetPosition).y + 2f;

        return targetPosition;
    }
}