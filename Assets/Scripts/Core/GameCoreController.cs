using QFramework;
using UnityEngine;

/// <summary>
/// ��Ϸ���Ŀ��������Զ����ɣ���������Mono��Ϊ
/// </summary>
public class GameCoreController : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}