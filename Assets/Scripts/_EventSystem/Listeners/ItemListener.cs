using _EventSystem.CustomEvents;
using _EventSystem.UnityEvents;
using Gears;

namespace _EventSystem.Listeners
{
    public class ItemListener : BaseGameEventListener<Gear, GearEvent, UnityItemEvent> { }
}
