using _EventSystem.CustomEvents;
using _EventSystem.UnityEvents;
using Units;

namespace _EventSystem.Listeners
{
    public class UnitListener : BaseGameEventListener<Unit, UnitEvent, UnityUnitEvent> { }
}