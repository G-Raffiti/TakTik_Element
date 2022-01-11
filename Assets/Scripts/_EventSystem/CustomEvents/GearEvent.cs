using Gears;
using UnityEngine;

namespace _EventSystem.CustomEvents
{
    [CreateAssetMenu(fileName = "New Item Event", menuName = "Game Events/Item Event")]
    public class GearEvent : BaseGameEvent<Gear> { }
}
