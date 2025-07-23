using UnityEngine;

public struct PlayerEnterInteractAreaEvent
{
    public IInteractable Interactable;
    public GameObject Player;
}

public struct PlayerExitInteractAreaEvent
{
    public IInteractable Interactable;
    public GameObject Player;
}

/// <summary>
/// 玩家交互事件
/// </summary>
public struct PlayerInteractEvent
{
    public IInteractable Interactable;
    public GameObject Player;
}

/// <summary>
/// 玩家交互后，状态立刻改变
/// </summary>
public struct InteractableStateChangedEvent
{
    public IInteractable Interactable;
    public bool IsActive;
}