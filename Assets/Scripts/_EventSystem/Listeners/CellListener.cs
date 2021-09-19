using _EventSystem.CustomEvents;
using _EventSystem.UnityEvents;
using Cells;

namespace _EventSystem.Listeners
{
    public class CellListener : BaseGameEventListener<Cell, CellEvent, UnityCellEvent> { }
}
