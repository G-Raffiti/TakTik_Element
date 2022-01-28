using _EventSystem.CustomEvents;
using StateMachine;
using TMPro;
using UnityEngine;

namespace UserInterface.BattleScene
{
    public class Battle_AP_MP_UI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI AP;
        [SerializeField] private TextMeshProUGUI MP;
        
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
            AP.text = "" + (int)BattleStateManager.instance.PlayingUnit.BattleStats.AP;
            MP.text = "" + (int)BattleStateManager.instance.PlayingUnit.BattleStats.MP;
        }

        public void OnEventRaised<T>(T item)
        {
            UpdateDisplay();
        }
    }
}