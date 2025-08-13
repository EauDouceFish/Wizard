using QFramework;
using UnityEngine;

public class ToggleGamePauseCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var gameCoreModel = this.GetModel<GameCoreModel>();

        // �л���ͣ״̬
        bool newPauseState = !gameCoreModel.IsGamePaused.Value;
        gameCoreModel.SetGamePaused(newPauseState);

        // ����Unityʱ������
        Time.timeScale = newPauseState ? 0f : 1f;

        // ������ͣ�¼�
        this.SendEvent(new GamePausedEvent { isPaused = newPauseState });

        Debug.Log($"��Ϸ��ͣ״̬�л�Ϊ: {newPauseState}");
    }
}