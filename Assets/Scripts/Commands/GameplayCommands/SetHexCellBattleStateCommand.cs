using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class SetHexCellBattleStateCommand : AbstractCommand
{
    HexCell hexCell;
    HexCellState state;
    GameEntityModel gameEntityModel;
    public SetHexCellBattleStateCommand(HexCell cell, HexCellState state)
    {
        this.hexCell = cell;
        this.state = state;
    }

    protected override void OnExecute()
    {
        gameEntityModel = this.GetModel<GameEntityModel>();



        this.SendEvent<OnHexCellBattleStateSetEvent>(  
            new OnHexCellBattleStateSetEvent(hexCell, state)
        );
    }
}

/// <summary>
/// ”…EntityModelπ‹¿Ì
/// </summary>
public struct OnHexCellBattleStateSetEvent
{
    public HexCell HexCell { get; private set; }
    public HexCellState State { get; private set; }
    public OnHexCellBattleStateSetEvent(HexCell hexCell, HexCellState state)
    {
        HexCell = hexCell;
        State = state;
    }
}