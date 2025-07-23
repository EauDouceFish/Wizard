using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class DivideHexMapStep : IMapGenerationStep
{
    int mapWidth;
    int mapHeight;

    public void Execute(MapModel mapModel)
    {
        mapWidth = mapModel.MapWidth;
        mapHeight = mapModel.MapHeight;
        GenerateMap(mapModel);
    }

    private void GenerateMap(MapModel mapModel)
    {
        if (mapWidth != 0 && mapHeight != 0)
        {
            // +2ȷ�����ɵ������ο���ȫ����
            // �ֱ����Ƶ���͸ߵĹ��ɼ���Ҫ����������
            int colHexNum = (int)(mapWidth / (mapModel.HexOuterRadius * 3 / 2) + 2);
            int rowHexNum = mapHeight / (int)(mapModel.HexInnerRadius * 2) + 2;

            Debug.Log($"MapWidth: {mapWidth}, MapHeight: {mapHeight}, col {colHexNum} row{rowHexNum}");

            mapModel.HexGrid.CreateHexGridXZ(colHexNum, rowHexNum);
        }
        else
        {
            Debug.LogWarning("Invalid MapSize");
        }
    }
}
