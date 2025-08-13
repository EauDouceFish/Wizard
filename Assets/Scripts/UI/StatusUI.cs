using System.Collections.Generic;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private Transform statusIconsContainer;
    [SerializeField] private GameObject statusIconPrefab;

    private List<StatusIcon> activeIcons = new();

    /// <summary>
    /// ˢ������Buffͼ��ͼ�ʱ��
    /// </summary>
    public void SetBuffs(List<BuffBase<Entity>> buffs)
    {
        // ��������icon��Ȼ����¡�����icon
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