using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

interface IEventHandler<T>
{
    void HandleEvent(T eventInfo);
}

abstract class EventRouter<T> : MonoBehaviour
{
    private HashSet<IEventHandler<T>> mEventHandlers = new HashSet<IEventHandler<T>>();
    private List<T> mPendingEvents = new List<T>();

    public void RegisterHandler(IEventHandler<T> listener)
    {
        mEventHandlers.Add(listener);
    }

    public void UnRegisterListener(IEventHandler<T> listener)
    {
        mEventHandlers.Remove(listener);
    }

    public void NotifyEvent(T eventInfo, bool async = true)
    {
        if (async)
        {
            mPendingEvents.Add(eventInfo);
        }
        else
        {
            NotifyListeners(eventInfo);
        }
    }

    private void NotifyListeners(T eventInfo)
    {
        foreach (IEventHandler<T> handler in mEventHandlers)
        {
            handler.HandleEvent(eventInfo);
        }
    }

    private void LateUpdate()
    {
        if (mPendingEvents.Count > 0)
        {
            List<T> pendingEvents = new List<T>(mPendingEvents);
            mPendingEvents.Clear();
            foreach (T pending in pendingEvents)
            {
                NotifyListeners(pending);
            }
        }
    }
}

