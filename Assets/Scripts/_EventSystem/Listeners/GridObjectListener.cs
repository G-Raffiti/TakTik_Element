using _EventSystem.CustomEvents;
using _EventSystem.UnityEvents;
using GridObjects;

namespace _EventSystem.Listeners
{
    public class GridObjectListener : BaseGameEventListener<GridObject, GridObjectEvent, UnityGridObjectEvent> { }
}