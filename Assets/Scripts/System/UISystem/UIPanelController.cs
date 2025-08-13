using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

public class UIPanelController : MonoBehaviour, IController
{
    [SerializeField] GameplayUI gameplayUI;
    [SerializeField] GamePauseUI gamePauseUI;
    UISystem UISystem;
    private void Awake()
    {
        UISystem = this.GetSystem<UISystem>();
    }

    private void OnEnable()
    {
        this.RegisterEvent<GamePausedEvent>(OnGamePausedEvent);
    }

    private void OnGamePausedEvent(GamePausedEvent evt)
    {
        if (evt.isPaused)
        {
            gamePauseUI.gameObject.SetActive(true);
        }
        else
        {
            gamePauseUI.gameObject.SetActive(false);
        }
    }


    #region Architecture

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
    #endregion
}