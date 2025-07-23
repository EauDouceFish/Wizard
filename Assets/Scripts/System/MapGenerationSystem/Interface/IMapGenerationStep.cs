using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapGenerationStep
{
    void Execute(MapModel mapModel);
}
