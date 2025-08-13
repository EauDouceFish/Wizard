using PlayerSystem;
using QFramework;
using System.Collections.Generic;
using UnityEngine;

public class MagicInputController : MonoBehaviour, IController, ICanSendEvent
{
    private MagicInputModel magicInputModel;
    private MagicSpellSystem magicSpellSystem;
    private Player player;

    // Ԫ�������λӳ�䣬���ﲻ��InputSystem������ڷ���
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
    /// ����Ԫ������
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
    /// ��Ŀ��ʩ����
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
            Debug.LogWarning("û��Ԫ�ؿ����ͷ�");
            return;
        }

        var spellInstance = magicSpellSystem.GetSpellToCast(currentElements);
        if (spellInstance is BasicSpellInstance basicSpell)
        {
            // ����ʩ��
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