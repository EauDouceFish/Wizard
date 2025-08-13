using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using QFramework;

public delegate bool ActorTypeFilter(Actor actor);

public class ActorManageSystem : AbstractSystem
{
    List<Actor> actorList = new List<Actor>();

    protected override void OnInit()
    {
    }

    public void RegisterActor(Actor actor)
    {
        if (actor == null || actor.gameObject == null)
        {
            Debug.LogWarning("Cannot register a null actor.");
            return;
        }

        if (!actorList.Contains(actor))
        {
            actorList.Add(actor);
            //Debug.Log($"Register Actor, Count: {actorList.Count}");
        }
    }

    public void UnregisterActor(Actor actor)
    {
        if (actor == null || actor.gameObject == null)
        {
            Debug.LogWarning("Cannot unregister a null or destroyed actor.");
            return;
        }
        if (actorList.Contains(actor))
        {
            actorList.Remove(actor);
            //Debug.Log($"Unregister Actor, Count: {actorList.Count}");
        }
    }

    #region 获取区域内指定Actor

    /// <summary>
    /// 获得管理器范围内的 可指定指定类型Actor
    /// </summary>
    /// <param name="me"></param>
    /// <param name="position"></param>
    /// <param name="range"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public List<Actor> GetActorsWithinViewRange(Actor me, Vector3 position, float range, ActorTypeFilter filter = null)
    {

        List<Actor> nearbyActors = new List<Actor>();
        foreach (Actor actor in actorList)
        {
            if (actor == me) continue;
            if (Vector3.Distance(actor.transform.position, position) <= range)
            {
                if (filter == null || filter(actor))
                {
                    nearbyActors.Add(actor);
                }
            }
        }

        return nearbyActors;
    }

    public List<T> GetActorsByType<T>() where T : Actor
    {
        List<T> typeActors = new List<T>();
        foreach (Actor actor in actorList)
        {
            if (actor is T)
            {
                typeActors.Add((T)actor);
            }
        }
        return typeActors;
    }

    public void ClearAllActors()
    {
        actorList.Clear();
    }

    protected override void OnDeinit()
    {
        base.OnDeinit();
        ClearAllActors();
    }


    #endregion
}
