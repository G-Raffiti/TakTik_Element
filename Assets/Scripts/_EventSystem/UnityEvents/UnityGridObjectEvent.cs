using System;
using GridObjects;
using UnityEngine.Events;

namespace _EventSystem.UnityEvents
{
    [Serializable] public class UnityGridObjectEvent : UnityEvent<GridObject> { }
}