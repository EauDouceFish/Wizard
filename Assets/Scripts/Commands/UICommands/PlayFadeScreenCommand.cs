using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class PlayFadeScreenCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogWarning("不存在Canvas，无法执行淡入命令");
            return;
        }
        GameObject fadeScreen = canvas.transform.Find("FadeScreen").transform.gameObject;
        if (fadeScreen)
        {
            fadeScreen.SetActive(true);
        }
        else
        {
            Debug.LogWarning("请添加淡入淡出UI GameObject");
        }
    }
}
