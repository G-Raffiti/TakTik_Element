using _EventSystem.CustomEvents;
using StateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UserInterface.BattleScene
{
    public class BattleAPMpUI : MonoBehaviour
    {
        [FormerlySerializedAs("AP")]
        [SerializeField] private TextMeshProUGUI ap;
        [FormerlySerializedAs("MP")]
        [SerializeField] private TextMeshProUGUI mp;
        
        [Header("Event Listener")]
        [SerializeField] private UnitEvent onStartTurn;
        [SerializeField] private VoidEvent onSkillUsed;
        [SerializeField] private UnitEvent onUnitMoved;
        [SerializeField] private VoidEvent onActionDone;

        private void OnEnable()
        {
            onStartTurn.EventListeners += OnEventRaised;
            onSkillUsed.EventListeners += OnEventRaised;
            onUnitMoved.EventListeners += OnEventRaised;
            onActionDone.EventListeners += OnEventRaised;

        }

        private void OnDisable()
        {
            onStartTurn.EventListeners -= OnEventRaised;
            onSkillUsed.EventListeners -= OnEventRaised;
            onUnitMoved.EventListeners -= OnEventRaised;
            onActionDone.EventListeners -= OnEventRaised;
        }

        public void UpdateDisplay()
        {
            ap.text = "" + (int)BattleStateManager.instance.PlayingUnit.battleStats.ap;
            mp.text = "" + (int)BattleStateManager.instance.PlayingUnit.battleStats.mp;
        }

        public void OnEventRaised<T>(T _item)
        {
            UpdateDisplay();
        }
    }
}