using PlayerSystem;
using QFramework;
using System.Collections.Generic;
using UnityEngine;

public class MagicInputController : MonoBehaviour, IController, ICanSendEvent
{
    private MagicInputModel magicInputModel;
    private MagicSpellSystem magicSpellSystem;
    private Player player;

    // 元素输入键位映射，这里不用InputSystem避免过于繁琐
    private readonly Dictionary<KeyCode, MagicElement> elementKeyMap = new()
    {
        { KeyCode.Q, MagicElement.Fire },
        { KeyCode.W, MagicElement.Water },
        { KeyCode.A, MagicElement.Ice },
        { KeyCode.S, MagicElement.Nature },
        { KeyCode.D, MagicElement.Rock }
    };

    private void Awake()
    {
        magicInputModel = this.GetModel<MagicInputModel>();
        magicSpellSystem = this.GetSystem<MagicSpellSystem>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        HandleElementInput();
        HandleSpellCasting();
    }

    /// <summary>
    /// 处理元素输入
    /// </summary>
    private void HandleElementInput()
    {
        foreach (var keyElementPair in elementKeyMap)
        {
            if (Input.GetKeyDown(keyElementPair.Key))
            {
                TryAddElement(keyElementPair.Value);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearElements();
        }
    }


    private void HandleSpellCasting()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    CastSpellOnSelf();
        //}
    }

    /// <summary>
    /// 向目标施法！
    /// </summary>
    public void OnLeftClickStarted()
    {
        var currentElements = magicInputModel.InputElements.Value;
        if (currentElements.Count == 0) return;

        var targetPosition = GOExtensions.GetMouseWorldPositionOnGround();
        var spellInstance = magicSpellSystem.GetSpellToCast(currentElements);

        if (spellInstance is BasicSpellInstance basicSpell)
        {
            if (basicSpell.GetSpellData().castType == CastType.Channel)
            {
                player.movementStateMachine.StartChanneling(basicSpell, targetPosition);
            }
            else
            {
                player.movementStateMachine.StartInstantCast(basicSpell, targetPosition);
            }


            basicSpell.Initialize(player, targetPosition, false, currentElements);
            ClearElements();
        }
    }

    public void OnLeftClickCanceled()
    {
        if (player.movementStateMachine.ReusableData.IsChanneling)
        {
            this.SendEvent(new OnChannelSpellCastEndedEvent { });
            player.movementStateMachine.StopChanneling();
        }
    }

    private void TryAddElement(MagicElement element)
    {
        if (magicInputModel.AvailableElements.Value.Contains(element))
        {
            magicInputModel.AddInputElement(element);

            magicSpellSystem.AddElement(element);
        }
    }

    private void ClearElements()
    {
        magicInputModel.ClearInputElements();
        magicSpellSystem.ClearElements();
    }

    private void CastSpellOnSelf()
    {
        var currentElements = magicInputModel.InputElements.Value;
        if (currentElements.Count == 0)
        {
            Debug.LogWarning("没有元素可以释放");
            return;
        }

        var spellInstance = magicSpellSystem.GetSpellToCast(currentElements);
        if (spellInstance is BasicSpellInstance basicSpell)
        {
            // 蓄力施法
            if (basicSpell.GetSpellData().castType == CastType.Channel)
            {

            }
            basicSpell.Initialize(player, player.transform.position, true, currentElements);
            basicSpell.Execute();
        }

        ClearElements();
    }

    public IArchitecture GetArchitecture() => GameCore.Interface;
}