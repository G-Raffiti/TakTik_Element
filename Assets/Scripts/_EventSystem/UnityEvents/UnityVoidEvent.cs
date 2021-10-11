using System;
using UnityEngine.Events;
using Void = _EventSystem.CustomEvents.Void;

namespace _EventSystem.UnityEvents
{
    [Serializable] public class UnityVoidEvent : UnityEvent<Void> { }
}