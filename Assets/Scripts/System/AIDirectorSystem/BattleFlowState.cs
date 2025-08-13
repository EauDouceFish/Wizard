
// ս������״̬��
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
        // ����ÿ��������������Ĳ��ַ������һ��
        enemiesPerWave = new List<int>();
        int remaining = config.totalEnemies;

        for (int i = 0; i < config.waveCount; i++)
        {
            if (i == config.waveCount - 1) // ���һ��
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