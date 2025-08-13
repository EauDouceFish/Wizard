
// 战斗流程状态类
using System.Collections.Generic;
using UnityEngine;

public class BattleFlowState
{
    public MonsterRegionConfig config;
    public int currentWave = 0;
    public List<int> enemiesPerWave;
    public Coroutine battleCoroutine;

    public BattleFlowState(MonsterRegionConfig config)
    {
        this.config = config;
        CalculateEnemiesPerWave();
    }

    private void CalculateEnemiesPerWave()
    {
        // 计算每波怪物数量，多的部分放在最后一波
        enemiesPerWave = new List<int>();
        int remaining = config.totalEnemies;

        for (int i = 0; i < config.waveCount; i++)
        {
            if (i == config.waveCount - 1) // 最后一波
            {
                enemiesPerWave.Add(remaining);
            }
            else
            {
                int enemiesThisWave = Mathf.Min(config.maxPerWave, remaining);
                enemiesPerWave.Add(enemiesThisWave);
                remaining -= enemiesThisWave;
            }
        }
    }
}