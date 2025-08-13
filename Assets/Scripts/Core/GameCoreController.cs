using QFramework;
using UnityEngine;

/// <summary>
/// 游戏核心控制器，自动生成，用来管理Mono行为
/// </summary>
public class GameCoreController : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}