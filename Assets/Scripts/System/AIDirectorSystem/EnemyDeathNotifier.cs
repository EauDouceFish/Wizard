using QFramework;
using UnityEngine;

/// <summary>
/// 监听由AIDirectorSystem产生的敌人死亡事件
/// </summary>
public class EnemyDeathNotifier : MonoBehaviour, IController, ICanSendEvent
{
    public HexCell belongToHexCell;

    // Destroy版本，后续考虑改为对象池版本
    private void OnDestroy()
    {
        if (belongToHexCell != null)
        {
            this.SendEvent(new EnemyDefeatedEvent
            {
                enemy = gameObject,
                hexCell = belongToHexCell
            });
            Debug.Log("检测到消灭了一个敌人");
        }
        else
        {
            //Debug.LogWarning("EnemyDeathNotifier:找不到敌人所属HexCell");
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}