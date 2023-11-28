using _EventSystem.CustomEvents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _EventSystem.Listeners
{
    /// <summary>
    /// GameEvent Listener as a MonoBehaviour
    /// </summary>
    /// <typeparam name="T">Tvpe of Data Sent</typeparam>
    /// <typeparam name="GE">GameEvent Type</typeparam>
    /// <typeparam name="UER">Unity Event</typeparam>
    public abstract class BaseGameEventListener<T, TGe, TUer> : MonoBehaviour
        where TGe : BaseGameEvent<T>
        where TUer : UnityEvent<T>
    {
        [FormerlySerializedAs("_GameEvent")]
        [SerializeField]
        protected TGe gameEvent;

        [FormerlySerializedAs("_UnityEventResponse")]
        [SerializeField]
        protected TUer unityEventResponse;

        protected void OnEnable()
        {
            if (gameEvent is null) return;
            gameEvent.EventListeners += TriggerResponses; // Subscribe
        }

        protected void OnDisable()
        {
            if (gameEvent is null) return;
            gameEvent.EventListeners -= TriggerResponses; // Unsubscribe
        }

        [ContextMenu("Trigger Responses")]
        public void TriggerResponses(T _val)
        {
            //No need to nullcheck here, UnityEvents do that for us (lets avoid the double nullcheck)
            unityEventResponse.Invoke(_val);
        }
    }
}