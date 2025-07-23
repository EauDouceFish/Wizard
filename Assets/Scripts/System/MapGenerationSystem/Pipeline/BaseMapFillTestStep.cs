using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMapFillTestStep : IMapGenerationStep
{
    public void Execute(MapModel mapModel)
    {
        Debug.Log("������ƽ��");

        int mapWidth = mapModel.MapWidth;
        int mapHeight = mapModel.MapHeight;

        GameObject baseMapPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        baseMapPlane.name = "BaseMapPlane";
        baseMapPlane.transform.localScale = new Vector3(mapWidth / 10f, 1f, mapHeight / 10f);

        // ����λ�õ���ͼ����

        baseMapPlane.transform.position = new Vector3((mapWidth - 1) * 0.5f, -0.2f, (mapHeight - 1) * 0.5f);
        Debug.Log($"������ {mapWidth}x{mapHeight}�Ļ���ƽ��");
    }
}