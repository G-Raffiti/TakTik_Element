using Cells;
using UnityEngine;

namespace _EventSystem.CustomEvents
{
    [CreateAssetMenu(fileName = "New Cell Event", menuName = "Game Events/Cell Event")]
    public class CellEvent : BaseGameEvent<Cell> { }
}
