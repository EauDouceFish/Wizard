using QFramework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ���Ƶ��õ�ͼ���ء������л�
/// </summary>
public class LoadingViewController : UIPanelBase
{
    public TextMeshProUGUI loadingText;
    public Image progressBar;

    private AsyncOperation operation;
    private int curProgressValue = 0;
    private bool isLoading = false;
    private float delay = 1.0f;

    /// <summary>
    /// ���ؽ�����Ϸ
    /// </summary>
    public void LoadGame()
    {
        StartCoroutine(LoadGameAsync());
    }

    /// <summary>
    /// �첽������Ϸ����
    /// </summary>
    private IEnumerator LoadGameAsync()
    {
        // ����
        this.SendCommand<PlayFadeScreenCommand>();
        yield return new WaitForSeconds(delay);

        /*var system = this.GetSystem<MapGenerationSystem>();
        system.ExecuteGenerationPipeline();*/
        yield return StartCoroutine(AsyncLoading(GameScene.PlayScene));
    }

    // �첽���ص�ͼ������ͬ����UI
    private IEnumerator AsyncLoading(GameScene scene)
    {
        operation = SceneManager.LoadSceneAsync(scene.ToString());

        //��ֹ����������Զ��л����ȴ��������
        operation.allowSceneActivation = false;
        isLoading = true;
        yield return operation;
    }

    private void Update()
    {
        // ֮ǰ�õĲ�����ٽ��������ȳ������ش˴����Զ��л�
        // �����пտ����ٰ�ʵ�ʽ���
        if (!isLoading)
        {
            return;
        }
        int progressValue = 100;
        if (curProgressValue < progressValue)
        {
            curProgressValue++;
        }
        loadingText.text = "��ͼ���ؽ��ȣ� " + curProgressValue + "%";
        progressBar.fillAmount = curProgressValue / 100f;
        if (curProgressValue == 100)
        {
            if (operation != null)
            {
                operation.allowSceneActivation = true;             
                isLoading = false;
            }
        }
    }
}
