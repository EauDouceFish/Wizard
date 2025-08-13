using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapCreateUI : UIPanelBase
{
    [SerializeField] private Button miniMapButton;
    [SerializeField] private Button smallMapButton;
    [SerializeField] private Button mediumMapButton;
    [SerializeField] private Button largeMapButton;
    [SerializeField] private MapSelectDescUI mapDescUI;

    // ���㴦��ť������ֵ䣬����������
    private Dictionary<Button, MapSize> buttonMap;


    protected override void Awake()
    {
        base.Awake();

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

            AddHoverEvents(kv.Key, kv.Value);
        }
    }

    private void AddHoverEvents(Button button, MapSize mapSize)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // �������¼�
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => {
            if (mapDescUI != null)
            {
                mapDescUI.UpdateMapDescription(mapSize);
            }
            else
            {
                Debug.LogWarning("����������UI���");
            }
        });
        trigger.triggers.Add(pointerEnter);
    }

    // Buttonѡ���ͼ��С���������ݲ���ִ������
    private void OnMapSizeSelected(MapSize size)
    {
        MapConfigurationManager.SetSelectedMapSize(size);
    }


    #region ��������
    /*
        private void SetMapSize(MapSize size)
        {
            mapModel.SetCurrentMapSize(size);
        }
    */
    #endregion
}
