using System.Collections.Generic;
using _EventSystem.CustomEvents;
using StateMachine;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using Void = _EventSystem.CustomEvents.Void;

namespace UserInterface.BattleScene
{
    public class BattleTurnOrderUI : MonoBehaviour
    {
        [SerializeField] private GameObject prefabUnitIcon;
        [FormerlySerializedAs("Turn")]
        [SerializeField] private TextMeshProUGUI turn;
        private Dictionary<Unit, GameObject> icons = new Dictionary<Unit, GameObject>();
        
        [Header("Event Listener")] 
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onGameStart;
        private void Start()
        {
            onGameStart.EventListeners += Initialize;
            onUnitStartTurn.EventListeners += UpdateDisplay;
        }

        private void OnDestroy()
        {
            onGameStart.EventListeners -= Initialize;
            onUnitStartTurn.EventListeners -= UpdateDisplay;
        }

        public void Initialize(Void _v)
        {
            BattleStateManager _cellGrid = BattleStateManager.instance; 
            turn.text = $"{_cellGrid.Turn}";
            turn.color = Color.white;
            foreach (Unit _unit1 in _cellGrid.Units)
            {
                Unit _unit = (Unit) _unit1;
                if (_unit == null) continue;
                GameObject _pref = Instantiate(prefabUnitIcon, transform);
                _pref.GetComponent<TurnOrderPrefab>().Initialize(_unit);
                _pref.transform.SetAsLastSibling();
                icons.Add(_unit, _pref);
            }
        }

        public void UpdateDisplay(Unit _u)
        {
            BattleStateManager _cellGrid = BattleStateManager.instance; 
            if (_cellGrid.NextCorruptionTurn > _cellGrid.Turn)
                turn.text = $"{_cellGrid.Turn} \n<size=20>Next Corruption in {_cellGrid.NextCorruptionTurn} Turn";
            else turn.text = $"{_cellGrid.Turn}";
            turn.color *= new Color(1, 0.97f, 0.97f);
            
            foreach (Unit _unit1 in _cellGrid.Units)
            {
                Unit _unit = _unit1;
                if (_unit == null) continue;
                icons[_unit].transform.SetAsLastSibling();
            }
            
            icons[_u].transform.SetSiblingIndex(0);
        }
    }
}