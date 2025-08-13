using QFramework;
using UnityEngine;

public class UnlockMagicElementCommand : AbstractCommand
{
  
    public MagicElement Element { get; }

    public UnlockMagicElementCommand(MagicElement element)
    {
        Debug.Log($"¼¤»îÔªËØ{element}");
        Element = element;
    }

    protected override void OnExecute()
    {
        var model = this.GetModel<MagicInputModel>();
        model.AddAvailableElement(Element);
        this.SendEvent<OnElementGetEvent>(new OnElementGetEvent { magicElement = Element });
    }
}
