using System;
using Skills;
using UnityEngine.Events;

namespace _EventSystem.UnityEvents
{
    [Serializable] public class UnitySkillEvent : UnityEvent<SkillInfo> { }
}