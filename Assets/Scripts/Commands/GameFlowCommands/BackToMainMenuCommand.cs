using QFramework;
using UnityEngine;

public class BackToMainMenuCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        SceneLoader.LoadScene(GameScene.MainMenuScene);
        GameCore.Interface.Deinit();

        GameCore.InitArchitecture();
    }
}