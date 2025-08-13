using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// ����ϵͳ��������Ϸ�ڵ��ߡ�ף���������޸�/�����޸ĵ�ϵͳ
/// </summary>
public class AttributeSystem : AbstractSystem
{
    private PropModel propModel;

    protected override void OnInit()
    {
        propModel = this.GetModel<PropModel>();
        this.RegisterEvent<GetHolyBlessingEvent>(OnGetHolyBlessingEvent);
        this.RegisterEvent<BlessPropSelectedEvent>(OnBlessPropSelectedEvent);
    }

    #region �¼�
    private void OnGetHolyBlessingEvent(GetHolyBlessingEvent evt)
    {
        // ҵ���߼������ѡ�����
        List<AbstractBlessingProp> availableProps = propModel.GetRandomBlessingPropsFiltered(1, 3);
        propModel.SetCurrentAvailableProps(availableProps);

        // ����UI�����¼�
        this.SendEvent(new OnAvailableBlessPropsAssignedEvent { availableProps = availableProps });
    }

    /// <summary>
    /// UIѡ��ף������ʱ�����
    /// </summary>
    public void OnBlessPropSelectedEvent(BlessPropSelectedEvent evt)
    {
        var selectedProp = evt.SelectedProp;
        selectedProp.Execute();
    }
    #endregion
}
