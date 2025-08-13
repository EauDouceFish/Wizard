using QFramework;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class TestManager : MonoBehaviour, IController
{
    // ��ӻ�Ԫ��Buff
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
        Debug.Log($"��Ϊ {count} ��Buffableʵ�������FireStatusBuff");
    }

    public void KillAllEnemies()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        int count = 0;

        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                Debug.Log($"��ɱ����: {enemy.name}");
                enemy.TakeDamage(99999999);
                count++;
            }
        }
        Debug.Log($"ɱ���� {count} ��Enemy");
    }

    // ��ȡ����Ԫ������
    public void GetAllElements()
    {
        // ��ȡ����MagicElementö��ֵ
        var allElements = Enum.GetValues(typeof(MagicElement)).Cast<MagicElement>().ToList();

        Debug.Log($"��ʼ��ȡ����Ԫ������: {string.Join(", ", allElements)}");

        // ����ElementPillar��ʵ�֣�����UnlockMagicElementCommand
        foreach (var element in allElements)
        {
            this.SendCommand(new UnlockMagicElementCommand(element));
        }

        Debug.Log("�ѻ����������Ԫ��������");
    }

    public void IncreaseDifficulty(int level = 1)
    {
        this.SendCommand(new IncreaseDifficultyCommand(level));

        var gameCoreModel = this.GetModel<GameCoreModel>();
        Debug.Log($"TestManager: ��ǰ��Ϸ�Ѷ�Ϊ {gameCoreModel.DifficultyLevel.Value}");
    }

    public void DecreaseDifficulty(int level = 1)
    {
        this.SendCommand(new DecreaseDifficultyCommand(level));

        var gameCoreModel = this.GetModel<GameCoreModel>();
        Debug.Log($"TestManager: ��ǰ��Ϸ�Ѷ�Ϊ {gameCoreModel.DifficultyLevel.Value}");
    }


    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}

public static class TestManagerMenu
{
    [MenuItem("DebugMode/TestManager/������Buffableʵ���FireStatusBuff")]
    public static void AddFireStatusBuffToAllEntities()
    {
        var testManager = GameObject.FindObjectOfType<TestManager>();
        if (testManager != null)
        {
            testManager.AddFireStatusBuffToAllEntities();
        }
        else
        {
            Debug.LogWarning("������û��TestManager���");
        }
    }

    [MenuItem("DebugMode/TestManager/ɱ�����е���")]
    public static void KillAllEnemies()
    {
        var testManager = GameObject.FindObjectOfType<TestManager>();
        if (testManager != null)
        {
            testManager.KillAllEnemies();
        }
        else
        {
            Debug.LogWarning("������û��TestManager���");
        }
    }

    [MenuItem("DebugMode/TestManager/�������Ԫ������")]
    public static void GetAllElements()
    {
        var testManager = GameObject.FindObjectOfType<TestManager>();
        if (testManager != null)
        {
            testManager.GetAllElements();
        }
        else
        {
            Debug.LogWarning("������û��TestManager���");
        }
    }

    [MenuItem("DebugMode/TestManager/�Ѷȿ���/�����Ѷ� +1")]
    public static void IncreaseDifficulty()
    {
        var testManager = GameObject.FindObjectOfType<TestManager>();
        if (testManager != null)
        {
            testManager.IncreaseDifficulty(1);
        }
        else
        {
            Debug.LogWarning("������û��TestManager���");
        }
    }

    [MenuItem("DebugMode/TestManager/�Ѷȿ���/�����Ѷ� -1")]
    public static void DecreaseDifficulty()
    {
        var testManager = GameObject.FindObjectOfType<TestManager>();
        if (testManager != null)
        {
            testManager.DecreaseDifficulty(1);
        }
        else
        {
            Debug.LogWarning("������û��TestManager���");
        }
    }
}
#endif