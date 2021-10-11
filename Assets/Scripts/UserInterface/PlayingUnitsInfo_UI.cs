using _EventSystem.CustomEvents;
using Grid;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class PlayingUnitsInfo_UI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI AP;
        [SerializeField] private TextMeshProUGUI APshadow;
        [SerializeField] private TextMeshProUGUI MP;
        [SerializeField] private TextMeshProUGUI MPshadow;
        
        [Header("Event Listener")]
        [SerializeField] private UnitEvent onStartTurn;
        [SerializeField] private VoidEvent onSkillUsed;
        [SerializeField] private UnitEvent onUnitMoved;

        private void OnEnable()
        {
            onStartTurn.EventListeners += OnEventRaised;
            onSkillUsed.EventListeners += OnEventRaised;
            onUnitMoved.EventListeners += OnEventRaised;
        }

        private void OnDisable()
        {
            onStartTurn.EventListeners -= OnEventRaised;
            onSkillUsed.EventListeners -= OnEventRaised;
            onUnitMoved.EventListeners -= OnEventRaised;
        }

        public void UpdateDisplay()
        {
            AP.text = "" + (int)BattleStateManager.instance.PlayingUnit.BattleStats.AP;
            APshadow.text = "" + (int)BattleStateManager.instance.PlayingUnit.BattleStats.AP;
            MP.text = "" + (int)BattleStateManager.instance.PlayingUnit.BattleStats.MP;
            MPshadow.text = "" + (int)BattleStateManager.instance.PlayingUnit.BattleStats.MP;
        }

        public void OnEventRaised<T>(T item)
        {
            UpdateDisplay();
        }
    }
}