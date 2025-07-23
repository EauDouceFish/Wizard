using QFramework;
using UnityEngine;

public interface IOutlineSystem : ISystem
{
    void ShowOutline(GameObject target, Color? color = null);
    void HideOutline(GameObject target);
    void SetOutlineColor(GameObject target, Color color);
}