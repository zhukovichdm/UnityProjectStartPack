using System;
using System.Collections.Generic;
using UnityEngine;

public class MyAction
{
    private readonly List<Action> actions = new List<Action>();
    public void Subscribe(Action action) => actions.Add(action);

    public void SubscribeUniquely(Action action)
    {
        if (actions.Contains(action) == false)
            actions.Add(action);
    }

    public void Unsubscribe(Action action) => actions.Remove(action);
    public void Publish() => actions.ForEach(action => action.Invoke());
    public void ClearListener() => actions.Clear();
    public int GetCount() => actions.Count;
}

public class MyAction<T>
{
    private readonly List<Action<T>> actions = new List<Action<T>>();
    public void Subscribe(Action<T> action) => actions.Add(action);

    public void SubscribeUniquely(Action<T> action)
    {
        if (actions.Contains(action) == false)
            actions.Add(action);
    }

    public void Unsubscribe(Action<T> action) => actions.Remove(action);
    public void Publish(T value) => actions.ForEach(action => action.Invoke(value));
    public void ClearListener() => actions.Clear();
    public int GetCount() => actions.Count;
}