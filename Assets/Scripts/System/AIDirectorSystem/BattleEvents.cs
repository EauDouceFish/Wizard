using UnityEngine;

/// <summary>
/// ��������¼�
/// </summary>
public struct WaveCompletedEvent
{
    public HexCell hexCell;
    public int currentWave;
    public int totalWaves;
}

/// <summary>
/// ��ҽ��������򣬴������¼�
/// </summary>
public struct OnRegionEnterTriggerEvent
{
    public HexCell hexCell;
}

/// <summary>
/// �����������¼�
/// </summary>
public struct OnBattleRegionClearedEvent
{
    public HexCell hexCell;
}

public struct OnRegionStateChangedEvent
{
    public HexCell hexCell;
    public HexCellState newState;
}

/// <summary>
/// ���������¼�
/// </summary>
public struct EnemyDefeatedEvent
{
    public GameObject enemy;
    public HexCell hexCell;
}


/// <summary>
/// ������ҵ�ָ��HexCell
/// </summary>
public struct TeleportToHexEvent
{
    public HexCell hexCell;
}