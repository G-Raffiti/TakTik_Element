using System.Collections.Generic;
using _EventSystem.Listeners;
using UnityEngine;
using UserInterface;

namespace _EventSystem.CustomEvents
{
    public abstract class BaseGameEvent<T> : ScriptableObject
    {
        private readonly List<IGameEventListener<T>> eventListeners = new List<IGameEventListener<T>>();

        public void Raise(T item)
        {
            for(int _i = eventListeners.Count - 1; _i >= 0; _i--)
                eventListeners[_i].OnEventRaised(item);
        }

        public void RegisterListener(IGameEventListener<T> listener)
        {
            if(!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(IGameEventListener<T> listener)
        {
            if(eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }
}
