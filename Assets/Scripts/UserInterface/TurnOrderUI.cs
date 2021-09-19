using System;
using System.Collections.Generic;
using Grid;
using TMPro;
using Units;
using UnityEngine;

namespace UserInterface
{
    public class TurnOrderUI : MonoBehaviour
    {
        [SerializeField] private GameObject prefabUnitIcon;
        [SerializeField] private TextMeshProUGUI Turn;
        private Dictionary<Unit, GameObject> Icons = new Dictionary<Unit, GameObject>();

        private BattleStateManager cellGrid;

        private void Start()
        {
            cellGrid = GameObject.Find("CellGrid").GetComponent<BattleStateManager>();
        }

        public void Initialize()
        {
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

        public void UpdateDisplay()
        {
            if (cellGrid.NextCorruptionTurn > cellGrid.Turn)
                Turn.text = $"{cellGrid.Turn} \n<size=20>Next Corruption in {cellGrid.NextCorruptionTurn} Turn";
            else Turn.text = $"{cellGrid.Turn}";
            Turn.color *= new Color(1, 0.97f, 0.97f);
            
            foreach (Unit _unit1 in cellGrid.Units)
            {
                Unit _unit = (Unit) _unit1;
                if (_unit == null) continue;
                Icons[_unit].transform.SetAsLastSibling();
            }
        }
    }
}