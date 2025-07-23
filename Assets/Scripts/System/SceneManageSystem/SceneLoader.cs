using UnityEngine.SceneManagement;

/// <summary>
/// 如果需要用到LoadingScene，可以使用SceneLoader【待定】
/// </summary>
public static class SceneLoader
{
    public static GameScene TargetScene;

    /// <summary>
    /// 跳转到LoadingScene
    /// </summary>
    /// <param name="target"></param>
    public static void LoadScene(GameScene target)
    {
        TargetScene = target;
        SceneManager.LoadScene(GameScene.LoadingScene.ToString());
    }

    public static string GetSceneName(GameScene name)
    {
        return name.ToString();
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(TargetScene.ToString());
    }
}
