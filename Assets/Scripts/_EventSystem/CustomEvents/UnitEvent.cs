using Units;
using UnityEngine;

namespace _EventSystem.CustomEvents
{
    [CreateAssetMenu(fileName = "New Unit Event", menuName = "Game Events/Unit Event")]
    public class UnitEvent : BaseGameEvent<Unit> { }
}