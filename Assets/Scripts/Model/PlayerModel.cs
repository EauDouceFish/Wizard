using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEditor.ShaderGraph.Drawing;

public class PlayerModel : AbstractModel
{
    protected override void OnInit()
    {
    }

    #region Architecture

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }

    #endregion
}
