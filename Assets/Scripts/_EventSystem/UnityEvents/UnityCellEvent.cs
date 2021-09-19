using System;
using Cells;
using UnityEngine.Events;

namespace _EventSystem.UnityEvents
{
    [Serializable] public class UnityCellEvent : UnityEvent<Cell> { }
}