using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;

public class FillPropStep : IMapGenerationStep, ICanGetModel
{
    Storage storage;
    MapModel mapModel;
    GameEntityModel gameEntityModel;

    GameObject entityGroups;

    public void Execute(MapModel mapModel)
    {
        this.mapModel = mapModel;
        gameEntityModel = this.GetModel<GameEntityModel>();
        storage = mapModel.GetUtility<Storage>();
        
        GameObject pillarPrefab = storage.GetElementPillarPrefab();
        
        CreateEntityGroupContainer();
        
        
        
        FillPillars(mapModel, pillarPrefab);
        FillEntityGroups();
    }

    private void FillEntityGroups()
    {
        // ����ȥ�ص�λ�ü��ϣ�ʹ��Vector3Int���ٸ��������
        HashSet<Vector3Int> entityGroupPositions = new();

        Dictionary<Vector3Int, HexCell> posHexCellMap = new();

        // ��������HexCell�ռ�EntityGroup����λ��
        foreach (var cellPair in mapModel.HexGrid.allHexCellCoordDict)
        {
            HexCell cell = cellPair.Value;
            if (cell.isOccupied)
            {
                HashSet<Vector3Int> cellPositions = new();
                cell.GetEntityGroupXZPositions(cellPositions);

                // ��EntityGroupλ�ù�������������HexCell
                // 50% ���ʸ�������λ��
                foreach (var pos in cellPositions)
                {
                    if (!entityGroupPositions.Contains(pos))
                    {
                        entityGroupPositions.Add(pos);
                        posHexCellMap[pos] = cell;
                    }
                    else
                    {
                        if (Random.Range(0f, 1f) < 0.5f)
                        {
                            posHexCellMap[pos] = cell;
                        }
                    }
                }
            }
        }

        foreach (Vector3Int entityGroupPos in entityGroupPositions)
        {
            Vector3 worldPosition = new(entityGroupPos.x, entityGroupPos.y, entityGroupPos.z);
            Vector3 groundPosition = GOExtensions.GetGroundPosition(worldPosition);

            HexCell ownerCell = posHexCellMap[entityGroupPos];
            Vector3 offsetPos = ApplyRandomOffsetTowardsCell(groundPosition, ownerCell);
            EntityGroup entityGroup = CreateEntityGroupAtPosition(offsetPos);

            // ��EntityGroup�󶨵���Ӧ��HexCell��ͨ��Model����
            gameEntityModel.AddEntityGroupToHexCell(ownerCell, entityGroup);
        }
    }


    private void CreateEntityGroupContainer()
    {
        entityGroups = GameObject.Find("EntityGroups");
        if (entityGroups == null)
        {
            entityGroups = new GameObject("EntityGroups");
            entityGroups.transform.position = Vector3.zero;
        }
    }

    private EntityGroup CreateEntityGroupAtPosition(Vector3 position)
    {
        GameObject entityGroupPrefab = storage.GetEntityGroupPrefab();
        GameObject entityGroupObj = GameObject.Instantiate(entityGroupPrefab, position, Quaternion.identity);
        EntityGroup entityGroup = entityGroupObj.GetComponent<EntityGroup>();
        entityGroupObj.transform.SetParent(entityGroups.transform);
        ConfigureEntityGroup(entityGroup, position);

        return entityGroup;
    }

    private static void ConfigureEntityGroup(EntityGroup entityGroup, Vector3 position)
    {

    }

    // ���Ԫ����
    private static void FillPillars(MapModel mapModel, GameObject pillarPrefab)
    {
        foreach (HexRealm realm in mapModel.HexGrid.GetHexRealms())
        {
            HexCell centerCell = realm.GetInitHexCell();

            // ������λ������Ԫ����
            Vector3 centerPosition = centerCell.transform.position;
            centerPosition = GOExtensions.GetGroundPosition(centerPosition) - Vector3.up * 0.1f;
            GameObject pillarObj = Object.Instantiate(pillarPrefab, centerPosition, Quaternion.identity);

            ElementPillar pillar = pillarObj.GetComponent<ElementPillar>();
            if (pillar != null)
            {
                Biome realmBiome = realm.GetRealmBiome();
                if (realmBiome != null)
                {
                    pillar.element = realmBiome.MagicElement;
                }
            }
        }
    }

    // ���䡢ƫ��EntityGroup
    private Vector3 ApplyRandomOffsetTowardsCell(Vector3 originalPosition, HexCell ownerCell)
    {
        Vector3 cellCenter = ownerCell.transform.position;

        float offsetRate = Random.Range(0.4f, 0.7f);

        Vector3 offsetPosition = Vector3.Lerp(cellCenter, originalPosition, offsetRate);

        offsetPosition.y = GOExtensions.GetGroundPosition(offsetPosition).y;

        return offsetPosition;
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}