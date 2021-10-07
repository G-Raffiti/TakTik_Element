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
        public override void Use(Cell _cell, SkillInfo _skillInfo)
        {
            if (_cell.IsWalkable)
            {
                Teleport(_cell, _skillInfo.Unit);
            }
            else
            {
                List<Cell> Neighbours = _cell.Neighbours.Where(_neighbour => _neighbour.IsWalkable).ToList();
                Neighbours.Sort((c1,c2) => c1.GetDistance(_skillInfo.Unit.Cell).CompareTo(c2.GetDistance(_skillInfo.Unit.Cell)));
                
                if (Neighbours.Count >= 1)
                    Teleport(Neighbours[0], _skillInfo.Unit);
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
            return _cell.IsWalkable || _cell.Neighbours.Any(_neighbour => _neighbour.IsWalkable || _skillInfo.Unit.Cell == _cell);
        }
    }
}