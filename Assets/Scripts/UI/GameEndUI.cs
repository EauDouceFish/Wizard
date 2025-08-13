using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class GameEndUI : UIPanelBase
{
    [SerializeField] private GameObject gameWinEndFadeScreen;
    [SerializeField] private GameObject gameLoseEndFadeScreen;

    protected override void Start()
    {
        this.RegisterEvent<OnGameWinEndEvent>(OnGameWinEnd).UnRegisterWhenCurrentSceneUnloaded();
        this.RegisterEvent<OnPlayerDeadEvent>(OnGameLoseEnd).UnRegisterWhenCurrentSceneUnloaded();
        base.Start();
    }

    private void OnGameWinEnd(OnGameWinEndEvent e)
    {
        gameWinEndFadeScreen.SetActive(true);
    }

    private void OnGameLoseEnd(OnPlayerDeadEvent e)
    {
        gameLoseEndFadeScreen.SetActive(true);
    }

    public void BackToMainMenuScene()
    {
        this.SendCommand(new BackToMainMenuCommand());
    }
}
