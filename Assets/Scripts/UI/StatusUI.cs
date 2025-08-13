using System.Collections.Generic;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private Transform statusIconsContainer;
    [SerializeField] private GameObject statusIconPrefab;

    private List<StatusIcon> activeIcons = new();

    /// <summary>
    /// 刷新所有Buff图标和计时条
    /// </summary>
    public void SetBuffs(List<BuffBase<Entity>> buffs)
    {
        // 清理多余的icon，然后更新、创建icon
        for (int i = activeIcons.Count - 1; i >= buffs.Count; i--)
        {
            Destroy(activeIcons[i].gameObject);
            activeIcons.RemoveAt(i);
        }

        for (int i = 0; i < buffs.Count; i++)
        {
            StatusIcon icon;
            if (i < activeIcons.Count)
            {
                icon = activeIcons[i];
            }
            else
            {
                var go = Instantiate(statusIconPrefab, statusIconsContainer);
                icon = go.GetComponent<StatusIcon>();
                activeIcons.Add(icon);
            }
            icon.SetBuff(buffs[i]);
        }
    }
}