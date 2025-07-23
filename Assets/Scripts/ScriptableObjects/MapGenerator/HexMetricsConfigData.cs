using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HexMetricsConfigData", menuName = "MapGenerationSystem/GridMetricsConfigData")]
public class HexMetricsConfigData : ScriptableObject
{
    public float OuterRadius = 50f;
    public float InnerRadius => OuterRadius * 0.866025404f; // �����ڰ뾶��sqrt(3)/2��
}


