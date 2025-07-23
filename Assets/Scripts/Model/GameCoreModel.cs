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
    /// GameCoreModel�������Ѷ�Ψһ����
    /// </summary>
    /// <param name="level"></param>
    public void SetDifficulty(int level)
    {
        if (level <= 0)
        {
            Debug.LogWarning("�Ѷȵȼ��������0����");
        }
        DifficultyLevel = Mathf.Max(1, level);
    }
}
