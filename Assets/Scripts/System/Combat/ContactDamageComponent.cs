using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����Ӵ�ʱ��������˺�����ʩ��ˮƽ����
/// </summary>
public class ContactDamageComponent : MonoBehaviour
{
    [SerializeField] private float damageAmount = 5f;
    [SerializeField] private float damageInterval = 0.3f;
    [SerializeField] private LayerMask targetLayerMask = -1;

    private Dictionary<Entity, float> lastDamageTime = new();
    public bool IsActive { get; private set; } = true;

    public void SetActive(bool active)
    {
        IsActive = active;
        if (!active) lastDamageTime.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActive) return;

        if ((targetLayerMask.value & (1 << other.gameObject.layer)) == 0) return;

        Entity entity = other.GetComponent<Entity>();
        if (entity == null || entity == GetComponent<Entity>()) return;

        if (lastDamageTime.ContainsKey(entity) && Time.time - lastDamageTime[entity] < damageInterval) return;

        entity.TakeDamage((int)damageAmount);
        lastDamageTime[entity] = Time.time;
    }

    /// <summary>
    /// ���Ŀ��һֱ�ڷ�Χ�ڣ����������˺���ͨ�����ᣩ
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (!IsActive) return;

        if ((targetLayerMask.value & (1 << other.gameObject.layer)) == 0) return;

        Entity entity = other.GetComponent<Entity>();
        if (entity == null || entity == GetComponent<Entity>()) return;

        if (!lastDamageTime.ContainsKey(entity) || Time.time - lastDamageTime[entity] >= damageInterval)
        {
            entity.TakeDamage((int)damageAmount);
            lastDamageTime[entity] = Time.time;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null && lastDamageTime.ContainsKey(entity))
        {
            lastDamageTime.Remove(entity);
        }
    }
}