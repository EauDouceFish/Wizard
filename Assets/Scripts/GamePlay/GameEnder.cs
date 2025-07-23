using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEnder : MonoBehaviour
{
    public static event Action OnGameEnd;

    [SerializeField] private bool requirePlayerTag = true;

    void Start()
    {
        // ȷ����Collider���������Ϊ������
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }
        col.isTrigger = true;
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // �����Ҫ���Player��ǩ
        if (requirePlayerTag && !other.CompareTag("Player"))
            return;

        // ������Ϸ�����¼�
        OnGameEnd?.Invoke();
    }
}