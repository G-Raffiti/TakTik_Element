using Gears;
using UnityEngine;

namespace _EventSystem.CustomEvents
{
    [CreateAssetMenu(fileName = "New Gear Info Event", menuName = "Game Events/Gear Info Event")]
    public class GearInfoEvent : BaseGameEvent<GearInfo> { }
}
