using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// ��ʼ��Ⱥϵ��ʼλ��
/// </summary>
public class InitBiomeStep : IMapGenerationStep
{
    public void Execute(MapModel mapModel)
    {
        HexCell[] cells = GetValidHexCells(mapModel);

        if (cells.Length == 0)
        {
            Debug.LogWarning("û���ҵ���Ч��HexCell��ʹ��������ӳ�ʼ��Ⱥϵ��������йؿ���������");
            cells = mapModel.HexGrid.GetRandomCells(mapModel.BiomeConfigData.Count);
        }

        List<BiomeSO> shuffledBiomes = new List<BiomeSO>(mapModel.BiomeConfigData);
        ShuffleBiomeList(shuffledBiomes);

        for (int i = 0; i < mapModel.MapTargetBiomeCount; i++)
        {
            BiomeSO biomeData = shuffledBiomes[i];
            Biome biome = new(biomeData);
            HexCell initCell = cells[i];

            // ��ʼ���Ӹ�����
            MeshRenderer renderer = initCell.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.SetFloat("_Metallic", 1.0f);
            }
            //Debug.Log($"Initializing Biome: {biome.BiomeName} at position ({initCell.coord.x}, {initCell.coord.y})");

            mapModel.HexGrid.CreateHexRealm(initCell, biome);
        }
    }


    /// <summary>
    /// Fisher-Yates��O��n��ϴ��
    /// </summary>
    private void ShuffleBiomeList(List<BiomeSO> biomeList)
    {
        for (int i = biomeList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            BiomeSO temp = biomeList[i];
            biomeList[i] = biomeList[randomIndex];
            biomeList[randomIndex] = temp;
        }
    }

    /// <summary>
    /// ���Count���Ϸ���HexCell����ʱ������������
    /// </summary>
    /// <param name="mapModel"></param>
    /// <returns></returns>
    private HexCell[] GetValidHexCells(MapModel mapModel)
    {
        int targetCellNum = mapModel.MapTargetBiomeCount;

        var dict = mapModel.HexGrid.allHexCellCoordDict;

        HexCell[] result = new HexCell[targetCellNum];

        if (dict.Count < targetCellNum)
        {
            return new HexCell[0];
        }

        // ��ȡ����HexCell�������ѡ
        List<HexCell> allCells = new List<HexCell>(dict.Values);

        int loopCount = 0; 
        int selectedCount = 0;
        while (selectedCount < targetCellNum)
        {
            //ÿ�����ѡһ����Valid�ͼ���select����һ����ֱ�Ӽ��룩
            int randomIndex = Random.Range(0, allCells.Count);
            HexCell candidate = allCells[randomIndex];
            if (selectedCount == 0)
            {
                result[0] = candidate;
                selectedCount++;
                allCells.RemoveAt(randomIndex);
            }
            else
            {
                // ����Ƿ�Valid����ʽΪ |x2 - x1| *0.5 + |y2 - y1| > 2
                int x1 = result[selectedCount - 1].coord.x;
                int y1 = result[selectedCount - 1].coord.y;
                int x2 = candidate.coord.x;
                int y2 = candidate.coord.y;

                bool distanceValid = (Mathf.Abs(x2 - x1) * 0.5f + Mathf.Abs(y2 - y1)) > 2;
                bool noCenterNearby = true;
                // �����ΧһȦ6�������Ƿ�������result��Ԫ��
                foreach (HexCell otherInitCells in result)
                {
                    if (otherInitCells == null) continue;
                    // �ж��Ƿ���candidate���ڣ�������candidate�Ƿ�������if����
                    List<Pos2D> neighborCoords = HexMetrics.GetHexNeighbourCoords(candidate);
                    foreach (Pos2D neighborCoord in neighborCoords)
                    {
                        HexCell neighborCell = mapModel.HexGrid.GetCellByCoord(neighborCoord);
                        for (int i = 0; i < selectedCount; i++)
                        {
                            if (result[i] == neighborCell)
                            {
                                noCenterNearby = false;
                                break;
                            }
                        }
                    }
                }

                if (distanceValid && noCenterNearby)
                {
                    result[selectedCount] = candidate;
                    selectedCount++;
                    allCells.RemoveAt(randomIndex);
                }
                // ������
            }

            // ��ֹ��ѡ��Զ�޷���������ѭ��
            loopCount++;
            if (loopCount > 2000) break;
        }
        if (loopCount > 2000)
        {
            Debug.LogWarning("���޶��������޷��ҵ��㹻�ĺϷ�HexCell����Ĳ��������ؿգ������ü�������ɣ��з���");
            return new HexCell[0];   
        }
        else
        {
            Debug.Log("InitBiomeStep���ҵ��˺Ϸ�����ʼ��");
            return result;
        }
    }
}
