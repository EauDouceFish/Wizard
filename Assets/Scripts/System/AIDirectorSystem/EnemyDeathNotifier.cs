using QFramework;
using UnityEngine;

/// <summary>
/// ������AIDirectorSystem�����ĵ��������¼�
/// </summary>
public class EnemyDeathNotifier : MonoBehaviour, IController, ICanSendEvent
{
    public HexCell belongToHexCell;

    // Destroy�汾���������Ǹ�Ϊ����ذ汾
    private void OnDestroy()
    {
        if (belongToHexCell != null)
        {
            this.SendEvent(new EnemyDefeatedEvent
            {
                enemy = gameObject,
                hexCell = belongToHexCell
            });
            Debug.Log("��⵽������һ������");
        }
        else
        {
            //Debug.LogWarning("EnemyDeathNotifier:�Ҳ�����������HexCell");
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}