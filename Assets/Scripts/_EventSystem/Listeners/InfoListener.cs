using _EventSystem.CustomEvents;
using _EventSystem.UnityEvents;
using Resources.ToolTip.Scripts;

namespace _EventSystem.Listeners
{
    public class InfoListener : BaseGameEventListener<IInfo, InfoEvent, UnityInfoEvent> { }
}
