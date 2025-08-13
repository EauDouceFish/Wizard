using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;
using QFramework;

public struct OnGameWinEndEvent
{
}

public class GameEnder : InteractableEntity
{
    public override void OnInteract(GameObject interactor)
    {
        this.SendEvent(new OnGameWinEndEvent());
    }
}