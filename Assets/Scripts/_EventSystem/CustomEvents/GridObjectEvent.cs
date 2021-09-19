using GridObjects;
using UnityEngine;

namespace _EventSystem.CustomEvents
{
    [CreateAssetMenu(fileName = "New GridObject Event", menuName = "Game Events/GridObject Event")]
    public class GridObjectEvent : BaseGameEvent<GridObject>
    {
    }
}