using QFramework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 控制调用地图加载、场景切换
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
    /// 加载进入游戏
    /// </summary>
    public void LoadGame()
    {
        StartCoroutine(LoadGameAsync());
    }

    /// <summary>
    /// 异步加载游戏场景
    /// </summary>
    private IEnumerator LoadGameAsync()
    {
        // 白屏
        this.SendCommand<PlayFadeScreenCommand>();
        yield return new WaitForSeconds(delay);

        /*var system = this.GetSystem<MapGenerationSystem>();
        system.ExecuteGenerationPipeline();*/
        yield return StartCoroutine(AsyncLoading(GameScene.PlayScene));
    }

    // 异步加载地图场景、同步给UI
    private IEnumerator AsyncLoading(GameScene scene)
    {
        operation = SceneManager.LoadSceneAsync(scene.ToString());

        //阻止当加载完成自动切换，等待加载完成
        operation.allowSceneActivation = false;
        isLoading = true;
        yield return operation;
    }

    private void Update()
    {
        // 之前用的播放虚假进度条，等场景加载此处就自动切换
        // 后续有空可以再绑定实际进度
        if (!isLoading)
        {
            return;
        }
        int progressValue = 100;
        if (curProgressValue < progressValue)
        {
            curProgressValue++;
        }
        loadingText.text = "地图加载进度： " + curProgressValue + "%";
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
