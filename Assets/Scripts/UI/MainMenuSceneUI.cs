using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

public class MainMenuSceneUI : UIPanelBase
{

    // ��������Ϸ
    public void NewGame()
    {
        this.SendCommand<StartGameCommand>();
        //StartCoroutine(LoadSceneWithFadeEffect(delay));
    }

    // �˳���Ϸ
    public void ExitGame()
    {
        Debug.Log("�˳���Ϸ");
        Application.Quit();
    }

    /*// ʵ�ּ���ҳ��ʱ�ĵ���̸��Ч��
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
            Debug.LogWarning("����ӵ��뵭��UI GameObject");
        }
    }*/
}
