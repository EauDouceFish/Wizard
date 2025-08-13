using QFramework;
using UnityEngine;

/// <summary>
/// �����Σ�ԭ������ÿ��HexCell���ĺͶ����м�����һ����Դ�С�ĵ���ģ�ͣ����Ҹ����������ת��������ɫ
/// </summary>
public class GroundModelFillStep : IMapGenerationStep
{
    Storage storage;

    public void Execute(MapModel mapModel)
    {
        storage = mapModel.GetUtility<Storage>();
        GameObject[] grounds = storage.GetAllGroundModels();

        var hexCells = mapModel.HexGrid.allHexCellCoordDict.Values;

        // ����GroundGroup
        GameObject groundGroup = GameObject.Find("Ground");
        if (groundGroup == null)
        {
            groundGroup = new GameObject("Ground");
            groundGroup.transform.position = Vector3.zero;
        }

        foreach (HexCell hexCell in hexCells)
        {
            if(!hexCell.isOccupied) continue; 

            Material cellMaterial = hexCell.HexRealm.GetRealmBiome().CommonMaterial;

            Vector3[] groundPos = hexCell.GetCornerPositionSemi();

            // ����߿�
            for (int i = 0; i < groundPos.Length; i++)
            {
                Vector3 pos = groundPos[i];
                // ���ѡ�����ģ��
                GameObject selectedGround = grounds[Random.Range(0, grounds.Length)];
                Vector3 offset = selectedGround.GetModelGeometryOffsetPos();
                GameObject groundModel = Object.Instantiate(selectedGround, pos + offset, Quaternion.identity);
                groundModel.transform.SetParent(groundGroup.transform);

                groundModel.SetMaterial(cellMaterial);


                // �����ת
                float randomYRotation = Random.Range(0f, 360f);
                groundModel.transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);
                groundModel.transform.localScale *= mapModel.ScaleParam;
            }

            // �������Ŀ�
            GameObject centerGround = grounds[Random.Range(0, grounds.Length)];
            Vector3 centerOffset = centerGround.GetModelGeometryOffsetPos();
            GameObject centerModel = Object.Instantiate(centerGround, hexCell.transform.position + centerOffset, Quaternion.identity);
            
            centerModel.SetMaterial(cellMaterial);

            // �����ת+����
            float centerRotation = Random.Range(0f, 360f);
            centerModel.transform.rotation = Quaternion.Euler(0f, centerRotation, 0f);
            centerModel.transform.SetParent(groundGroup.transform);
            float randomScale = Random.Range(1.3f, 1.5f);
            centerModel.transform.localScale *= randomScale;
            centerModel.transform.localScale *= mapModel.ScaleParam;
        }
    }

}
