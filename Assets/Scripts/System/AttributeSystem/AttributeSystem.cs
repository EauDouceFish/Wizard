using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// 属性系统，处理游戏内道具、祝福，属性修改/其他修改的系统
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

    #region 事件
    private void OnGetHolyBlessingEvent(GetHolyBlessingEvent evt)
    {
        // 业务逻辑：随机选择道具
        List<AbstractBlessingProp> availableProps = propModel.GetRandomBlessingPropsFiltered(1, 3);
        propModel.SetCurrentAvailableProps(availableProps);

        // 发送UI更新事件
        this.SendEvent(new OnAvailableBlessPropsAssignedEvent { availableProps = availableProps });
    }

    /// <summary>
    /// UI选择祝福道具时候调用
    /// </summary>
    public void OnBlessPropSelectedEvent(BlessPropSelectedEvent evt)
    {
        var selectedProp = evt.SelectedProp;
        selectedProp.Execute();
    }
    #endregion
}
