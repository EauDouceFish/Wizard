using PlayerSystem;
using QFramework;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    [SerializeField] EnemyUI enemyUI;

    protected override void InitHealthUI()
    {
        UISystem uiSystem = this.GetSystem<UISystem>();
        HealthUI healthUI = uiSystem.FindUIPanelBase<EnemyUI>().bossHealthUI;
        BindHealthUI(healthUI);
        healthUI.gameObject.SetActive(true);
    }


    protected override void InitStatusUI()
    {
        GameObject statusUIPrefab = storage.GetStatusUIPrefab();
        if (statusUIPrefab)
        {
            GameObject statusUIObject = Instantiate(statusUIPrefab, transform);
            statusUIObject.transform.localPosition = new Vector3(0, 5f, 0);
            statusUI = statusUIObject.GetComponent<StatusUI>();
            BindStatusUI(statusUI);
        }
    }

    protected override void Dead()
    {
        if (statusUI)
        {
            statusUI.gameObject.SetActive(false);
        }
        this.SendCommand(new KillAllEndHexCellEnemiesCommand());
        this.SendCommand(new SummonGameEnderCommand());
        if (enemyData.deathVFX != null)
        {
            Instantiate(enemyData.deathVFX, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}