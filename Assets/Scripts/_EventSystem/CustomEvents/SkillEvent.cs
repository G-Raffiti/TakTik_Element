using Skills;
using UnityEngine;

namespace _EventSystem.CustomEvents
{
    [CreateAssetMenu(fileName = "New Skill Event", menuName = "Game Events/Skill Event")]
    public class SkillEvent : BaseGameEvent<SkillInfo> { }
}