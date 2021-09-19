using System;
using System.Collections.Generic;
using System.Linq;
using Cells;
using Units;
using UnityEngine;

namespace Grid.UnitGenerators
{
    public class CustomUnitGenerator : MonoBehaviour, IUnitGenerator
    {
        public Transform unitsParent;
        public Transform cellsParent;

        /// <summary>
        /// Returns units that are already children of UnitsParent object.
        /// </summary>
        public List<Unit> SpawnUnits(List<Cell> cells)
        {
            unitsParent = GameObject.Find("Units").transform;
            List<Unit> _ret = new List<Unit>();
            for (int _i = 0; _i < unitsParent.childCount; _i++)
            {
                Unit _unit = unitsParent.GetChild(_i).GetComponent<Unit>();
                if (_unit != null)
                {
                    Cell _cell = cells.OrderBy(h => Math.Abs((h.transform.position - _unit.transform.position).magnitude)).First();
                    {
                        _cell.Take(_unit);
                        _unit.transform.position = _cell.transform.position;
                        _unit.Initialize();
                        _ret.Add(_unit);
                    }//Unit gets snapped to the nearest cell
                }
                else
                {
                    Debug.LogError("Invalid object in Units Parent game object");
                }

            }
            return _ret;
        }

        /// <summary>
        /// Snaps unit objects to the nearest cell.
        /// </summary>
        public void SnapToGrid()
        {
            List<Transform> _cells = new List<Transform>();

            foreach (Transform _cell in cellsParent)
            {
                _cells.Add(_cell);
            }

            foreach (Transform _unit in unitsParent)
            {
                Transform _closestCell = _cells.OrderBy(h => Math.Abs((h.transform.position - _unit.transform.position).magnitude)).First();
                if (!_closestCell.GetComponent<Cell>().isTaken)
                {
                    Vector3 _offset = new Vector3(0, _closestCell.GetComponent<Cell>().GetCellDimensions().y, 0);
                    _unit.localPosition = _closestCell.transform.localPosition + _offset;
                }//Unit gets snapped to the nearest cell
            }
        }
    }
}
