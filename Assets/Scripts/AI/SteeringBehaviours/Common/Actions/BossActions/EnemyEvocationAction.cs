using System.Collections;
using UnityEngine;
using QFramework;
using BehaviorDesigner.Runtime.Tasks;
using PlayerSystem;

public class EnemyEvocationAction : EnemyAction, IController
{
    [Header("�ٻ�����")]
    public float minSummonNum = 3f;
    public float maxSummonNum = 5f;

    private MapModel mapModel;
    private bool hasEvoked = false;

    public override void OnStart()
    {
        base.OnStart();
        SetAnimationTrigger();

        mapModel = this.GetModel<MapModel>();
        StartCoroutine(PerformEvocation());
    }

    public override TaskStatus OnUpdate()
    {
        if (hasEvoked)
        {
            hasEvoked = false;
            SetAnimationBool(false);
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    private IEnumerator PerformEvocation()
    {
        SetAnimationBool(true);
        yield return new WaitForSeconds(5.0f);

        HexCell bossCurrentHexCell = GetBossCurrentHexCell();

        int summonCount = Mathf.RoundToInt(Random.Range(minSummonNum, maxSummonNum));

        this.SendCommand(new SummonRandomEnemiesAtHexCellCommand(bossCurrentHexCell, summonCount));

        //Debug.Log($"Boss�ٻ� {summonCount} ������");
        hasEvoked = true;
    }
    private HexCell GetBossCurrentHexCell()
    {
        foreach (var kvp in mapModel.HexGrid.allHexCellCoordDict)
        {
            HexCell cell = kvp.Value;
            if (cell.isEndLocation)
            {
                return cell;
            }
        }
        Debug.LogWarning("δ�ҵ� BossHexCell ���ڵ� HexCell");
        return null;
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}