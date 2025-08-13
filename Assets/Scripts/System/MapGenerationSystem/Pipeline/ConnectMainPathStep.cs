using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// ��ʼ��Ⱥϵ��ʼλ��
/// </summary>
public class ConnectMainPathStep : IMapGenerationStep, ICanGetModel
{
    private MapModel mapModel;

    /// <summary>
    /// ����˳������ÿ������
    /// </summary>
    public void Execute(MapModel mapModel)
    {
        this.mapModel = mapModel;
        List<HexRealm> hexRealms = mapModel.HexGrid.GetHexRealms();
        for (int i = 0; i < hexRealms.Count-1; i++)
        {
            Path mainPath = ConnectHexRealmSymmetrically(hexRealms[i], hexRealms[i + 1]);
            foreach (HexCell cell in mainPath.hexCells)
            {
                cell.isMainPath = true;
                cell.ShowIsMainPathText();
                // ��ӵ���·��
                mapModel.MainPath.Add(cell);
            }
        }
    }

    /// <summary>
    /// ������������ĳ�ʼλ�ã�������Path�������HexCell
    /// </summary>
    public Path ConnectHexRealmSymmetrically(HexRealm hexRealm1, HexRealm hexRealm2)
    {
        if (mapModel == null)
        {
            mapModel = this.GetModel<MapModel>();
        }
        // ��ȡ���������ʼ����ͨ·��
        HexCell initCell1 = hexRealm1.GetInitHexCell();
        HexCell initCell2 = hexRealm2.GetInitHexCell();
        Path path = mapModel.HexGrid.GetHexCellPathFinder().FindPath(initCell1, initCell2);
        PathFinderHelper.SetRoadForPath(path);
        //VisualizePath(path, Color.yellow);
        // ModifyCircleSummonAvailablePos(path); ����Group
        //Debug.Log($"������ {initCell1.HexRealm.GetRealmBiome().BiomeName} �� {initCell2.HexRealm.GetRealmBiome().BiomeName} ��ͨ·��������Ϊ{path.hexCells.Length}");
        int left = 0;
        int right = path.hexCells.Length - 1;
        while (left <= right)
        {
            // ������ͬ�����ѡ�񣬸��Ӳ�ͬ�ͷֱ�ֵ����������
            if (left == right)
            {
                int ramdomChoose = Random.Range(0, 2);
                if (ramdomChoose == 0)
                {
                    hexRealm1.AddHexCellIntoRealm(path.hexCells[left]);
                }
                else
                {
                    hexRealm2.AddHexCellIntoRealm(path.hexCells[left]);
                }
            }
            else
            {
                hexRealm1.AddHexCellIntoRealm(path.hexCells[left]);
                hexRealm2.AddHexCellIntoRealm(path.hexCells[right]);
            }

            left++;
            right--;
        }
        return path;
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
