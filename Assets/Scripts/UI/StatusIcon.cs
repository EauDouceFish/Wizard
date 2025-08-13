using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour
{
    [SerializeField] public Image statusIcon;
    [SerializeField] public Image statusTimerIcon;

    private BuffBase<Entity> buff;

    public void SetBuff(BuffBase<Entity> buff)
    {
        this.buff = buff;
        statusIcon.sprite = Resources.Load<Sprite>(buff.BuffData.iconPath);
        statusIcon.enabled = true;
        UpdateTimer();
    }

    private void Update()
    {
        if (buff != null && !buff.BuffData.isPermanent)
        {
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        float percent = Mathf.Clamp01(buff.ResidualDuration / buff.BuffData.maxDuration);
        statusTimerIcon.fillAmount = percent;
    }
}
