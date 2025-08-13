using QFramework;
using UnityEngine;

/// <summary>
/// 填充地形，原理是在每个HexCell中心和顶点中间生成一个相对大小的地形模型，并且给它随机的旋转、附上颜色
/// </summary>
public class GroundModelFillStep : IMapGenerationStep
{
    Storage storage;

    public void Execute(MapModel mapModel)
    {
        storage = mapModel.GetUtility<Storage>();
        GameObject[] grounds = storage.GetAllGroundModels();

        var hexCells = mapModel.HexGrid.allHexCellCoordDict.Values;

        // 创建GroundGroup
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

            // 处理边块
            for (int i = 0; i < groundPos.Length; i++)
            {
                Vector3 pos = groundPos[i];
                // 随机选择地面模型
                GameObject selectedGround = grounds[Random.Range(0, grounds.Length)];
                Vector3 offset = selectedGround.GetModelGeometryOffsetPos();
                GameObject groundModel = Object.Instantiate(selectedGround, pos + offset, Quaternion.identity);
                groundModel.transform.SetParent(groundGroup.transform);

                groundModel.SetMaterial(cellMaterial);


                // 随机旋转
                float randomYRotation = Random.Range(0f, 360f);
                groundModel.transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);
                groundModel.transform.localScale *= mapModel.ScaleParam;
            }

            // 处理中心块
            GameObject centerGround = grounds[Random.Range(0, grounds.Length)];
            Vector3 centerOffset = centerGround.GetModelGeometryOffsetPos();
            GameObject centerModel = Object.Instantiate(centerGround, hexCell.transform.position + centerOffset, Quaternion.identity);
            
            centerModel.SetMaterial(cellMaterial);

            // 随机旋转+缩放
            float centerRotation = Random.Range(0f, 360f);
            centerModel.transform.rotation = Quaternion.Euler(0f, centerRotation, 0f);
            centerModel.transform.SetParent(groundGroup.transform);
            float randomScale = Random.Range(1.3f, 1.5f);
            centerModel.transform.localScale *= randomScale;
            centerModel.transform.localScale *= mapModel.ScaleParam;
        }
    }

}
