using _EventSystem.CustomEvents;
using UnityEngine;
using UnityEngine.Events;

namespace _EventSystem.Listeners
{
    /// <summary>
    /// GameEvent Listener as a MonoBehaviour
    /// </summary>
    /// <typeparam name="T">Tvpe of Data Sent</typeparam>
    /// <typeparam name="GE">GameEvent Type</typeparam>
    /// <typeparam name="UER">Unity Event</typeparam>
    public abstract class BaseGameEventListener<T, GE, UER> : MonoBehaviour
        where GE : BaseGameEvent<T>
        where UER : UnityEvent<T>
    {
        [SerializeField]
        protected GE _GameEvent;

        [SerializeField]
        protected UER _UnityEventResponse;

        protected void OnEnable()
        {
            if (_GameEvent is null) return;
            _GameEvent.EventListeners += TriggerResponses; // Subscribe
        }

        protected void OnDisable()
        {
            if (_GameEvent is null) return;
            _GameEvent.EventListeners -= TriggerResponses; // Unsubscribe
        }

        [ContextMenu("Trigger Responses")]
        public void TriggerResponses(T val)
        {
            //No need to nullcheck here, UnityEvents do that for us (lets avoid the double nullcheck)
            _UnityEventResponse.Invoke(val);
        }
    }
}