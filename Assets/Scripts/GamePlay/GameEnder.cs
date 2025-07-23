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
        // 确保有Collider组件并设置为触发器
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
        // 如果需要检查Player标签
        if (requirePlayerTag && !other.CompareTag("Player"))
            return;

        // 触发游戏结束事件
        OnGameEnd?.Invoke();
    }
}