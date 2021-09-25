using _EventSystem.CustomEvents;
using _EventSystem.Listeners;
using Grid;
using TMPro;
using Units;
using UnityEngine;
using Void = _EventSystem.Void;

namespace UserInterface
{
    public class PlayingUnitsInfo_UI : MonoBehaviour, IGameEventListener<Unit>, IGameEventListener<Void>
    {
        [SerializeField] private TextMeshProUGUI AP;
        [SerializeField] private TextMeshProUGUI APshadow;
        [SerializeField] private TextMeshProUGUI MP;
        [SerializeField] private TextMeshProUGUI MPshadow;

        [SerializeField] private UnitEvent onStartTurn;
        [SerializeField] private VoidEvent onSkillUsed;
        [SerializeField] private UnitEvent onUnitMoved;

        private void Start()
        {
            onStartTurn.RegisterListener(this);
            onSkillUsed.RegisterListener(this);
            onUnitMoved.RegisterListener(this);
        }

        public void UpdateDisplay()
        {
            AP.text = "" + (int)BattleStateManager.instance.PlayingUnit.BattleStats.AP;
            APshadow.text = "" + (int)BattleStateManager.instance.PlayingUnit.BattleStats.AP;
            MP.text = "" + (int)BattleStateManager.instance.PlayingUnit.BattleStats.MP;
            MPshadow.text = "" + (int)BattleStateManager.instance.PlayingUnit.BattleStats.MP;
        }

        public void OnEventRaised(Unit item)
        {
            UpdateDisplay();
        }

        public void OnEventRaised(Void item)
        {
            UpdateDisplay();
        }

        private void OnDestroy()
        {
            onStartTurn.UnregisterListener(this);
            onSkillUsed.UnregisterListener(this);
            onUnitMoved.UnregisterListener(this);
        }
    }
}