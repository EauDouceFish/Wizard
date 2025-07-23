using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class GameEntry : MonoBehaviour
{
    private void Awake()
    {
        GameCore.InitArchitecture();
    }
}
