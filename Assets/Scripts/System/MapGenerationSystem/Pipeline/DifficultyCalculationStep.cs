using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 从最长边开始，用Queue记录权重递推计算，直到Queue为空
/// 计算方式：拿到所有和当前路径相邻的HexCell，每个HexCell难度都是周围路径计算的平均值+0.5
/// </summary>
//public class DifficultyCalculationStep : IMapGenerationStep
//{
//    public void Execute(MapModel mapModel)
//    {
//        List<HexCell> mainPath = mapModel.MainPath;
//        for (int i = 1; i <= mainPath.Count; i++)
//        {
//            int x = i;
//            mainPath[i-1].difficultyText.text = x.ToString();
//        }
//    }
//}