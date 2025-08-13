using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.UI;

public class MinimapHex : MonoBehaviour, IController
{
    [SerializeField] Image hexImage;
    [SerializeField] Image infoImage;

    public Button button;
    private HexCell representingHexCell;
    private bool teleportable = false;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            TryTeleportToRepresentingHexCell();
        });
    }

    public void SetColorOpaque(Color color)
    {
        Color newColor = new(color.r, color.g, color.b, 1f);
        hexImage.color = newColor;
    }

    public void SetInfoSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            infoImage.color = new Color(255, 255, 255, 0);
            return;
        }
        infoImage.sprite = sprite;
        var color = infoImage.color;
        color.a = 1f;
        infoImage.color = color;
    }

    public void SetRepresentingHexCell(HexCell hexCell)
    {
        representingHexCell = hexCell;
    }

    public void SetTeleportable()
    {
        this.teleportable = true;
    }

    public void SetCompleteColor()
    {
        float alpha = hexImage.color.a;
        Color completeColor = new(1f, 1f, 1f, alpha);
        hexImage.color = completeColor;
        teleportable = true;
    }
    /// <summary>
    /// 六边形外接圆半径
    /// </summary>
    public float GetHexOuterRadius()
    {
        return gameObject.GetComponent<RectTransform>().sizeDelta.x / 2;
    }

    private void TryTeleportToRepresentingHexCell()
    {
        if (teleportable)
        {
            this.SendCommand(new TPPlayerCommand(representingHexCell));
        }
        else
        {
            Debug.Log("这个MinimapHex暂时不是可传送的，请先设置SetTeleportable");
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
