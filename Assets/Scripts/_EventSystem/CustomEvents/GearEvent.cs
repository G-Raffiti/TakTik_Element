using Gears;
using UnityEngine;

namespace _EventSystem.CustomEvents
{
    [CreateAssetMenu(fileName = "New Gear Event", menuName = "Game Events/Gear Event")]
    public class GearEvent : BaseGameEvent<Gear> { }
}
