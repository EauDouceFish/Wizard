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
            Debug.LogWarning("������Canvas���޷�ִ�е�������");
            return;
        }
        GameObject fadeScreen = canvas.transform.Find("FadeScreen").transform.gameObject;
        if (fadeScreen)
        {
            fadeScreen.SetActive(true);
        }
        else
        {
            Debug.LogWarning("����ӵ��뵭��UI GameObject");
        }
    }
}
