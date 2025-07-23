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
/// ��ҽ����¼�
/// </summary>
public struct PlayerInteractEvent
{
    public IInteractable Interactable;
    public GameObject Player;
}

/// <summary>
/// ��ҽ�����״̬���̸ı�
/// </summary>
public struct InteractableStateChangedEvent
{
    public IInteractable Interactable;
    public bool IsActive;
}