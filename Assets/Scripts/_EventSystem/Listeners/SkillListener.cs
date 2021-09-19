using _EventSystem.CustomEvents;
using _EventSystem.UnityEvents;
using Skills;

namespace _EventSystem.Listeners
{
    public class SkillListener : BaseGameEventListener<SkillInfo, SkillEvent, UnitySkillEvent> { }
}