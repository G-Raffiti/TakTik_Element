using UnityEngine;

namespace _EventSystem.CustomEvents
{
    [CreateAssetMenu(fileName = "New Bool Event", menuName = "Game Events/Bool Event")]
    public class BoolEvent : BaseGameEvent<bool> { }
}
