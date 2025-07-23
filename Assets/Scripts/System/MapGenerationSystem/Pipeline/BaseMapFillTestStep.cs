using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMapFillTestStep : IMapGenerationStep
{
    public void Execute(MapModel mapModel)
    {
        Debug.Log("填充基本平面");

        int mapWidth = mapModel.MapWidth;
        int mapHeight = mapModel.MapHeight;

        GameObject baseMapPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        baseMapPlane.name = "BaseMapPlane";
        baseMapPlane.transform.localScale = new Vector3(mapWidth / 10f, 1f, mapHeight / 10f);

        // 设置位置到地图中心

        baseMapPlane.transform.position = new Vector3((mapWidth - 1) * 0.5f, -0.2f, (mapHeight - 1) * 0.5f);
        Debug.Log($"生成了 {mapWidth}x{mapHeight}的基础平面");
    }
}