using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class SpellInstanceBase : MonoBehaviour, IController
{
    protected SpellBaseData SpellBaseData;

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
