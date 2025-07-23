using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class Debugger
{
    private static bool logEnabled = true; // Change to static  
    public static void Log(object message)
    {
        if (logEnabled)
        {
            Debug.Log(message);
        }
    }

    public static void Log(object message, Object context)
    {
        if (logEnabled)
        {
            Debug.Log(message, context);
        }
    }
}

/*  private static Stopwatch TestConsumption()
{
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    // ģ����������
    double dummy = 0;
    for (int i = 0; i < 30000000; i++)  // 3ǧ��μ���
    {
        dummy += Mathf.Sqrt(Random.Range(0, 1000));
    }

    stopwatch.Stop();
    return stopwatch;
}*/