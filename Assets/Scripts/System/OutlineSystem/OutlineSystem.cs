using QFramework;
using System.Collections.Generic;
using UnityEngine;

public class OutlineSystem : AbstractSystem, IOutlineSystem
{
    /// <summary>
    /// 维护所有可交互物体的OutlineController
    /// </summary>
    private Dictionary<GameObject, OutlineController> outlineControllers = new();
    private Color defaultOutlineColor = new Color(0.53f, 0.8f, 0.98f, 1f);
    private Material outlineMaterial;
    private Storage storage;
    protected override void OnInit()
    {
        storage = this.GetUtility<Storage>();
        outlineMaterial = storage.GetOutlineMaterial();
        // 后续可以拉入luban导表系统去单独设置每个Color，在这里可以初始化ItemColor或者区域Color，现在先不管暂时统一了
    }

    /// <summary>
    /// 获取传入目标的OutlineController
    /// </summary>
    /// <param name="target"></param>
    /// <param name="color"></param>
    public void ShowOutline(GameObject target, Color? color = null)
    {
        if (target == null) return;

        OutlineController controller = GetOrAddOutlineController(target);
        if (controller != null)
        {
            if (color.HasValue)
            {
                SetOutlineColor(target, color.Value);
            }
            controller.SetOutlineEnabled(true);
        }
    }

    /// <summary>
    /// 隐藏传入目标的OutlineController
    /// </summary>
    public void HideOutline(GameObject target)
    {
        if (target == null) return;

        if (outlineControllers.TryGetValue(target, out var controller))
        {
            controller.SetOutlineEnabled(false);
        }
    }

    /// <summary>
    /// 传入可描边物体，设置目标的Outline颜色
    /// </summary>
    /// <param name="target"></param>
    /// <param name="color"></param>
    public void SetOutlineColor(GameObject target, Color color)
    {
        if (target == null)
        {
            Debug.LogWarning("目标对象为null，无法设置轮廓颜色");
            return;
        }

        OutlineController controller = GetOrAddOutlineController(target);
        controller.SetOutlineColor(color);
    }

    public Material GetOutlineMaterial()
    {
        return outlineMaterial;
    }
    /// <summary>
    /// 获取目标的Outline控制器，没有就自动添加一个新的，以后不需要手动添加
    /// </summary>
    /// <param name="interactableObject"></param>
    /// <returns></returns>
    private OutlineController GetOrAddOutlineController(GameObject interactableObject)
    {
        if (outlineControllers.TryGetValue(interactableObject, out OutlineController controller))
        {
            return controller;
        }

        controller = interactableObject.GetComponent<OutlineController>();

        if (controller == null)
        {
            controller = interactableObject.AddComponent<OutlineController>();
            controller.SetOutlineColor(defaultOutlineColor);
            outlineControllers[interactableObject] = controller;
        }
        return controller;
    }

}