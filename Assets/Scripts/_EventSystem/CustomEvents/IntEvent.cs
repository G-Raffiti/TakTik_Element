using UnityEngine;

namespace _EventSystem.CustomEvents
{
    [CreateAssetMenu(fileName = "New Int Event", menuName = "Game Events/Int Event")]
    public class IntEvent : BaseGameEvent<int> { }
}
