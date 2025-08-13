using QFramework;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class TestManager : MonoBehaviour, IController
{
    // 添加火元素Buff
    public void AddFireStatusBuffToAllEntities()
    {
        var allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
        int count = 0;
        foreach (MonoBehaviour mb in allMonoBehaviours)
        {
            if (mb is IBuffableEntity buffable)
            {
                buffable.BuffSystem.AddBuff<FireStatus>("TestManager", 1);
                count++;
            }
        }
        Debug.Log($"已为 {count} 个Buffable实体添加了FireStatusBuff");
    }

    public void KillAllEnemies()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        int count = 0;

        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                Debug.Log($"击杀敌人: {enemy.name}");
                enemy.TakeDamage(99999999);
                count++;
            }
        }
        Debug.Log($"杀死了 {count} 个Enemy");
    }

    // 获取所有元素力量
    public void GetAllElements()
    {
        // 获取所有MagicElement枚举值
        var allElements = Enum.GetValues(typeof(MagicElement)).Cast<MagicElement>().ToList();

        Debug.Log($"开始获取所有元素力量: {string.Join(", ", allElements)}");

        // 仿造ElementPillar的实现，发送UnlockMagicElementCommand
        foreach (var element in allElements)
        {
            this.SendCommand(new UnlockMagicElementCommand(element));
        }

        Debug.Log("已获得所有五种元素力量！");
    }

    public void IncreaseDifficulty(int level = 1)
    {
        this.SendCommand(new IncreaseDifficultyCommand(level));

        var gameCoreModel = this.GetModel<GameCoreModel>();
        Debug.Log($"TestManager: 当前游戏难度为 {gameCoreModel.DifficultyLevel.Value}");
    }

    public void DecreaseDifficulty(int level = 1)
    {
        this.SendCommand(new DecreaseDifficultyCommand(level));

        var gameCoreModel = this.GetModel<GameCoreModel>();
        Debug.Log($"TestManager: 当前游戏难度为 {gameCoreModel.DifficultyLevel.Value}");
    }


    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}

public static class TestManagerMenu
{
    [MenuItem("DebugMode/TestManager/给所有Buffable实体加FireStatusBuff")]
    public static void AddFireStatusBuffToAllEntities()
    {
        var testManager = GameObject.FindObjectOfType<TestManager>();
        if (testManager != null)
        {
            testManager.AddFireStatusBuffToAllEntities();
        }
        else
        {
            Debug.LogWarning("场景中没有TestManager组件");
        }
    }

    [MenuItem("DebugMode/TestManager/杀死所有敌人")]
    public static void KillAllEnemies()
    {
        var testManager = GameObject.FindObjectOfType<TestManager>();
        if (testManager != null)
        {
            testManager.KillAllEnemies();
        }
        else
        {
            Debug.LogWarning("场景中没有TestManager组件");
        }
    }

    [MenuItem("DebugMode/TestManager/获得所有元素力量")]
    public static void GetAllElements()
    {
        var testManager = GameObject.FindObjectOfType<TestManager>();
        if (testManager != null)
        {
            testManager.GetAllElements();
        }
        else
        {
            Debug.LogWarning("场景中没有TestManager组件");
        }
    }

    [MenuItem("DebugMode/TestManager/难度控制/增加难度 +1")]
    public static void IncreaseDifficulty()
    {
        var testManager = GameObject.FindObjectOfType<TestManager>();
        if (testManager != null)
        {
            testManager.IncreaseDifficulty(1);
        }
        else
        {
            Debug.LogWarning("场景中没有TestManager组件");
        }
    }

    [MenuItem("DebugMode/TestManager/难度控制/减少难度 -1")]
    public static void DecreaseDifficulty()
    {
        var testManager = GameObject.FindObjectOfType<TestManager>();
        if (testManager != null)
        {
            testManager.DecreaseDifficulty(1);
        }
        else
        {
            Debug.LogWarning("场景中没有TestManager组件");
        }
    }
}
#endif