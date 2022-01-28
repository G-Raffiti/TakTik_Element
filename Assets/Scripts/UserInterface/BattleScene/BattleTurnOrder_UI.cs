using System.Collections.Generic;
using _EventSystem.CustomEvents;
using StateMachine;
using TMPro;
using Units;
using UnityEngine;
using Void = _EventSystem.CustomEvents.Void;

namespace UserInterface.BattleScene
{
    public class BattleTurnOrder_UI : MonoBehaviour
    {
        [SerializeField] private GameObject prefabUnitIcon;
        [SerializeField] private TextMeshProUGUI Turn;
        private Dictionary<Unit, GameObject> Icons = new Dictionary<Unit, GameObject>();
        
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

        public void Initialize(Void v)
        {
            BattleStateManager cellGrid = BattleStateManager.instance; 
            Turn.text = $"{cellGrid.Turn}";
            Turn.color = Color.white;
            foreach (Unit _unit1 in cellGrid.Units)
            {
                Unit _unit = (Unit) _unit1;
                if (_unit == null) continue;
                GameObject _pref = Instantiate(prefabUnitIcon, transform);
                _pref.GetComponent<TurnOrderPrefab>().Initialize(_unit);
                _pref.transform.SetAsLastSibling();
                Icons.Add(_unit, _pref);
            }
        }

        public void UpdateDisplay(Unit u)
        {
            BattleStateManager cellGrid = BattleStateManager.instance; 
            if (cellGrid.NextCorruptionTurn > cellGrid.Turn)
                Turn.text = $"{cellGrid.Turn} \n<size=20>Next Corruption in {cellGrid.NextCorruptionTurn} Turn";
            else Turn.text = $"{cellGrid.Turn}";
            Turn.color *= new Color(1, 0.97f, 0.97f);
            
            foreach (Unit _unit1 in cellGrid.Units)
            {
                Unit _unit = _unit1;
                if (_unit == null) continue;
                Icons[_unit].transform.SetAsLastSibling();
            }
            
            Icons[u].transform.SetSiblingIndex(0);
        }
    }
}