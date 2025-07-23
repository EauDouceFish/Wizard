using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

public class UIPanelController : MonoBehaviour, IController
{
    [SerializeField] GameplayUI gameplayUI;

    UISystem UISystem;
    private void Awake()
    {
        UISystem = this.GetSystem<UISystem>();
    }

    #region Architecture

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
    #endregion
}