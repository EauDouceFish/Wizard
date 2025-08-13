using UnityEngine;

/// <summary>
/// 波次完成事件
/// </summary>
public struct WaveCompletedEvent
{
    public HexCell hexCell;
    public int currentWave;
    public int totalWaves;
}

/// <summary>
/// 玩家进入了区域，触发该事件
/// </summary>
public struct OnRegionEnterTriggerEvent
{
    public HexCell hexCell;
}

/// <summary>
/// 区域清剿完成事件
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
/// 敌人死亡事件
/// </summary>
public struct EnemyDefeatedEvent
{
    public GameObject enemy;
    public HexCell hexCell;
}


/// <summary>
/// 传送玩家到指定HexCell
/// </summary>
public struct TeleportToHexEvent
{
    public HexCell hexCell;
}