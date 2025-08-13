using UnityEngine;
using QFramework;
using System.Collections;
using UnityEngine.Rendering.VirtualTexturing;

public class StartGameCommand : AbstractCommand, ICanSendCommand
{
    protected override void OnExecute()
    {
        GameObject canvas = GOExtensions.GetCanvas();
        LoadingViewController loadingViewController = canvas.Find<LoadingViewController>("LoadingViewController");
        loadingViewController.gameObject.SetActive(true);
        loadingViewController.LoadGame();

        GameCore.Interface.Deinit();
        GameCore.InitArchitecture();
    }
}
