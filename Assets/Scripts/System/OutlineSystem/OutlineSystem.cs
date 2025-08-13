using QFramework;
using System.Collections.Generic;
using UnityEngine;

public class OutlineSystem : AbstractSystem, IOutlineSystem
{
    /// <summary>
    /// ά�����пɽ��������OutlineController
    /// </summary>
    private Dictionary<GameObject, OutlineController> outlineControllers = new();
    private Color defaultOutlineColor = new Color(0.53f, 0.8f, 0.98f, 1f);
    private Material outlineMaterial;
    private Storage storage;
    protected override void OnInit()
    {
        storage = this.GetUtility<Storage>();
        outlineMaterial = storage.GetOutlineMaterial();
        // ������������luban����ϵͳȥ��������ÿ��Color����������Գ�ʼ��ItemColor��������Color�������Ȳ�����ʱͳһ��
    }

    /// <summary>
    /// ��ȡ����Ŀ���OutlineController
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
    /// ���ش���Ŀ���OutlineController
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
    /// �����������壬����Ŀ���Outline��ɫ
    /// </summary>
    /// <param name="target"></param>
    /// <param name="color"></param>
    public void SetOutlineColor(GameObject target, Color color)
    {
        if (target == null)
        {
            Debug.LogWarning("Ŀ�����Ϊnull���޷�����������ɫ");
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
    /// ��ȡĿ���Outline��������û�о��Զ����һ���µģ��Ժ���Ҫ�ֶ����
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