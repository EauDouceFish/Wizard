using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class DiffuseCellsStep : IMapGenerationStep
{
    MapModel mapModel;
    HexGrid hexGrid;
    public void Execute(MapModel mapModel)
    {
        this.mapModel = mapModel;
        hexGrid = mapModel.HexGrid;
        DiffuseHexRealmCells();
    }

    // ������������ɢ��ֱ���ﵽ��ͼ��׼
    private void DiffuseHexRealmCells()
    {
        int mapNeedingCellNum = 50;  // ��ͼ��С��׼��Ҫ��ĸ���������δ����mapModel��ʼ��
        int currentHexCellSum = 0;

        var realms = mapModel.HexGrid.GetHexRealms();

        // ͳ��ƽ��ֵ
        foreach (HexRealm realm in realms)
        {
            currentHexCellSum += realm.GetHexRealmSize();
        }

        int averageHexCellNum;
        int whileCount = 0; // ������ѭ��������������û�й涨������ʼHexCellλ���£�ĳЩHexCell��360�Ȱ�Χ�������޷���ɢ
        while (currentHexCellSum < mapNeedingCellNum)
        {
            if (whileCount > 1000)
            {
                Debug.LogWarning("����ĵ�ͼ��ĳЩCell������������ޣ������޷����չ�������");
                break;
            }
            // ����HexRealmӵ��HexCell������ƽ��ֵ�������ǰ�����������ƽ��ֵ֮�£��ͽ��䳢����ɢ
            foreach (HexRealm realm in realms)
            {
                averageHexCellNum = currentHexCellSum / realms.Count;
                if (realm.GetHexRealmSize() <= averageHexCellNum)
                {
                    int expandNum = ExpandRealm(realm);
                    currentHexCellSum += expandNum;
                }
            }
            whileCount++;
        }
        //Debug.Log(currentHexCellSum);
    }

    // ������չĳ������n���ؿ�
    private int ExpandRealm(HexRealm hexRealm, int num = 1)
    {
        List<HexCell> allHexCells = hexRealm.GetHexCellsInRealm();
        int availableCellsFound = 0;

        for (int i = 0; i < num; i++)
        {
            HashSet<HexCell> candidateSet = new HashSet<HexCell>();
            Dictionary<HexCell, HexCell> expansionMapping = new Dictionary<HexCell, HexCell>();

            foreach (HexCell cell in allHexCells)
            {
                List<HexCell> neighbors = hexGrid.GetHexCellPathFinder().GetNeighborHexCells(cell);
                foreach (HexCell candidate in neighbors)
                {
                    if (!candidate.isOccupied)
                    {
                        candidateSet.Add(candidate);

                        // ��¼��չ��Դ
                        expansionMapping[candidate] = cell;
                    }
                }
            }

            if (candidateSet.Count == 0) return availableCellsFound;

            // ���ѡ��һ���ؿ������չ
            List<HexCell> availableCellsList = new List<HexCell>(candidateSet);
            int randomHexCellIdx = UnityEngine.Random.Range(0, availableCellsList.Count);
            HexCell newCell = availableCellsList[randomHexCellIdx];

            // ��ȡ�������չ�ĵؿ������ĸ��ɵؿ���չ�ġ�����Ϊ·
            HexCell sourceCell = expansionMapping[newCell];

            PathFinderHelper.SetRoadBetweenCells(sourceCell, newCell);

            // ��������
            hexRealm.AddHexCellIntoRealm(newCell);
            availableCellsFound++;
        }

        return availableCellsFound;
    }

}


