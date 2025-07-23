using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapCreateUI : MonoBehaviour, IController
{
    [SerializeField] private List<BiomeSO> biomeSOList;
    [SerializeField] private Button miniMapButton;
    [SerializeField] private Button smallMapButton;
    [SerializeField] private Button mediumMapButton;
    [SerializeField] private Button largeMapButton;

    private MapModel mapModel;

    // 方便处理按钮，添加字典，方便批处理
    private Dictionary<Button, MapSize> buttonMap;


    private void Awake()
    {
        mapModel = this.GetModel<MapModel>();

        buttonMap = new Dictionary<Button, MapSize>()
        {
            { miniMapButton, MapSize.Mini },
            { smallMapButton, MapSize.Small },
            { mediumMapButton, MapSize.Medium },
            { largeMapButton, MapSize.Large }
        };

        foreach (var kv in buttonMap)
        {
            kv.Key.onClick.AddListener(() =>
            {
                OnMapSizeSelected(kv.Value);
                this.SendCommand<StartGameCommand>();
            });
        }

        foreach (BiomeSO biomeSO in biomeSOList)
        {
            // 随机选择四个点位
        }
    }

    // Button选择地图大小后，设置数据并且执行生成
    private void OnMapSizeSelected(MapSize size)
    {
        mapModel.SetCurrentMapSize(size);
    }

    #region 辅助方法
/*
    private void SetMapSize(MapSize size)
    {
        mapModel.SetCurrentMapSize(size);
    }
*/
    #endregion
    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
