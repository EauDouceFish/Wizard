using UnityEngine.SceneManagement;

/// <summary>
/// �����Ҫ�õ�LoadingScene������ʹ��SceneLoader��������Ŀǰ������
/// </summary>
public static class SceneLoader
{
    public static GameScene TargetScene;

    /// <summary>
    /// ��ת��LoadingScene
    /// </summary>
    /// <param name="target"></param>
    public static void LoadScene(GameScene target)
    {
        TargetScene = target;
        SceneManager.LoadScene(target.ToString());
    }

    public static string GetSceneName(GameScene name)
    {
        return name.ToString();
    }

    //public static void LoaderCallback()
    //{
    //    SceneManager.LoadScene(TargetScene.ToString());
    //}
}
