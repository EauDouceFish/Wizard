using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

public class MainMenuSceneUI : UIPanelBase
{

    // 加载新游戏
    public void NewGame()
    {
        this.SendCommand<StartGameCommand>();
        //StartCoroutine(LoadSceneWithFadeEffect(delay));
    }

    // 退出游戏
    public void ExitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }

    /*// 实现加载页面时的淡入谈出效果
    private IEnumerator LoadSceneWithFadeEffect(float _delay)
    {
        TryPlayFadeScreen();
        yield return new WaitForSeconds(_delay);
        SceneManager.LoadScene(GameScene.PlayScene.ToString());
        Debug.Log($"Load {GameScene.PlayScene}");
    }

    private static void TryPlayFadeScreen()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject fadeScreen = canvas.transform.Find("FadeScreen").transform.gameObject;
        if (fadeScreen)
        {
            fadeScreen.SetActive(true);
        }
        else
        {
            Debug.LogWarning("请添加淡入淡出UI GameObject");
        }
    }*/
}
