using QFramework;
using UnityEngine;

public interface IInteractionSystem : ISystem
{
    void RegisterInteractable(IInteractable interactable);
    void UnregisterInteractable(IInteractable interactable);
    IInteractable GetCurrentInteractingTarget();
}