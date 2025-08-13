using QFramework;
using UnityEngine;

public class ToggleGamePauseCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var gameCoreModel = this.GetModel<GameCoreModel>();

        // 切换暂停状态
        bool newPauseState = !gameCoreModel.IsGamePaused.Value;
        gameCoreModel.SetGamePaused(newPauseState);

        // 设置Unity时间缩放
        Time.timeScale = newPauseState ? 0f : 1f;

        // 发送暂停事件
        this.SendEvent(new GamePausedEvent { isPaused = newPauseState });

        Debug.Log($"游戏暂停状态切换为: {newPauseState}");
    }
}