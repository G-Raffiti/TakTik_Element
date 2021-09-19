using Resources.ToolTip.Scripts;
using UnityEngine;

namespace _EventSystem.CustomEvents
{
    [CreateAssetMenu(fileName = "New Info Event", menuName = "Game Events/Info Event")]
    public class InfoEvent : BaseGameEvent<IInfo> { }
}
