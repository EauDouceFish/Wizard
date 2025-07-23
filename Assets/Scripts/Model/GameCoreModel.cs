using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCoreModel : AbstractModel
{
    public int DifficultyLevel { get; private set; } = 1;

    protected override void OnInit()
    {
    }

    /// <summary>
    /// GameCoreModel内设置难度唯一方法
    /// </summary>
    /// <param name="level"></param>
    public void SetDifficulty(int level)
    {
        if (level <= 0)
        {
            Debug.LogWarning("难度等级必须大于0！！");
        }
        DifficultyLevel = Mathf.Max(1, level);
    }
}
