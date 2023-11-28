using System.Collections.Generic;
using System.Linq;
using Cells;
using Units;
using UnityEngine;

namespace Skills.ScriptableObject_GridEffect
{
    [CreateAssetMenu(fileName = "GridEffect_Teleport_to", menuName = "Scriptable Object/Skills/Grid Effect Teleport to")]
    public class TeleportTo : SkillGridEffect
    {
        public override void Use(Cell _targetCell, SkillInfo _skillInfo)
        {
            if (_targetCell.IsWalkable)
            {
                Teleport(_targetCell, _skillInfo.unit);
            }
            else
            {
                List<Cell> _neighbours = _targetCell.Neighbours.Where(_neighbour => _neighbour.IsWalkable).ToList();
                _neighbours.Sort((_c1,_c2) => _c1.GetDistance(_skillInfo.unit.Cell).CompareTo(_c2.GetDistance(_skillInfo.unit.Cell)));
                
                if (_neighbours.Count >= 1)
                    Teleport(_neighbours[0], _skillInfo.unit);
            }
        }

        // TODO : create animation and transform it to an IEnumerator
        private static void Teleport(Cell _cell, Unit _unit)
        {
            _unit.Cell.FreeTheCell();
            _cell.Take(_unit);
            _unit.transform.position = _cell.transform.position;
            _unit.AutoSortOrder();
        }
        
        
        public override string InfoEffect()
        {
            return "Teleport to the Targeted Cell";
        }

        public override string InfoEffect(SkillInfo _skillInfo)
        {
            return InfoEffect();
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return _cell.IsWalkable || _cell.Neighbours.Any(_neighbour => _neighbour.IsWalkable || _skillInfo.unit.Cell == _cell);
        }
    }
}