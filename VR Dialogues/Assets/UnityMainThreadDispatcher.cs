using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private static readonly Queue<Action> _actionQueue = new Queue<Action>();

    public static UnityMainThreadDispatcher Instance()
    {
        if (_instance == null)
        {
            var obj = new GameObject("UnityMainThreadDispatcher");
            _instance = obj.AddComponent<UnityMainThreadDispatcher>();
            UnityEngine.Object.DontDestroyOnLoad(obj);
        }
        return _instance;
    }

    public void Enqueue(Action action)
    {
        lock (_actionQueue)
        {
            _actionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        while (_actionQueue.Count > 0)
        {
            lock (_actionQueue)
            {
                var action = _actionQueue.Dequeue();
                action.Invoke();
            }
        }
    }
}
