using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Instances;
using _ScriptableObject;
using Cells;
using Grid;
using Pathfinding.Algorithms;
using Skills._Zone;
using Stats;
using Units;
using UnityEngine;

namespace Skills.ScriptableObject_GridEffect
{
    [CreateAssetMenu(fileName = "GridEffect_Push", menuName = "Scriptable Object/Skills/Grid Effect Push Fix")]
    public class PushFix : SkillGridEffect
    {
        [SerializeField] private int Strength;
        
        public override void Use(Cell _cell, SkillInfo _skillInfo)
        {
            List<Cell> _zone = Zone.GetZone(_skillInfo.skill.Range, _cell);

            List<IMovable> affecteds = new List<IMovable>();
            foreach (Cell _cellAffected in _zone)
            {
                if (Zone.GetAffected(_cellAffected, _skillInfo.skill) != null)
                    affecteds.Add(Zone.GetAffected(_cellAffected, _skillInfo.skill));
            }

            if (affecteds.Count == 0) return;
            
            DataBase.RunCoroutine(Push(affecteds, _skillInfo.Unit.Cell, _skillInfo, Strength));
        }

        /// <summary>
        /// Coroutine that move 
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="_from"></param>
        /// <param name="skill"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public static IEnumerator Push(List<IMovable> targets, Cell _from, SkillInfo skill, int strength)
        {
            targets.Sort((target1, target2) =>
                target1.Cell.GetDistance(_from).CompareTo(target2.Cell.GetDistance(_from)));
            targets.Reverse();
            foreach (IMovable target in targets)
            {
                DataBase.RunCoroutine(Push(_from, skill, target, strength));
                while (target.IsMoving)
                {
                    yield return null;
                }
            }
        }

        public static IEnumerator Push(Cell _from, SkillInfo skill, IMovable target, int strength)
        {
            // find the target
            if (target == null) yield break;
            Cell _targetedCell = target.Cell;
            target.IsMoving = true;

            // find the destination
            Cell destination = target.Cell;

            for (int i = 1; i <= strength; i++)
            {
                Cell arrival = BattleStateManager.instance.Cells.Find(c =>
                    c.OffsetCoord == target.Cell.OffsetCoord + Zone.Direction(_from, _targetedCell) * i);
                if (arrival == null) break;
                destination = arrival;
            }
            
            
            // find the shortest path between target and destination
            Dictionary<Cell, Dictionary<Cell, float>> _edges = new Dictionary<Cell, Dictionary<Cell, float>>();
            foreach (Cell _cell in BattleStateManager.instance.Cells)
            {
                _edges[_cell] = new Dictionary<Cell, float>();
                foreach (Cell _neighbour in _cell.Neighbours)
                {
                    _edges[_cell][_neighbour] = 1;
                }
            }
            DijkstraPathfinding pathfinder = new DijkstraPathfinding();
            Dictionary<Cell, List<Cell>> _paths = pathfinder.FindAllPaths(_edges, _targetedCell);
            List<Cell> _path = _paths[destination];
            _path = _path.OrderBy(c => _targetedCell.GetDistance(c)).Reverse().ToList();

            // Move
            int _distance = Movable.Move(target, destination, _path).Count;
            if(_distance != 0)
                while (target.IsMoving) yield return null;
            
            Debug.Log($"Push : {skill.Unit.UnitName} pushed {target.getName()} of {_distance} Cells");
            
            // If an Unit hit an other object, both take damage
            if (_distance >= strength) yield break;
            Cell obstacleCell = BattleStateManager.instance.Cells.Find(c =>
                c.OffsetCoord == target.Cell.OffsetCoord + Zone.Direction(_from, _targetedCell));
            Unit obstacle = null;
            
            if (obstacleCell != null)
                 obstacle = obstacleCell.CurrentUnit;
            
            if (target == obstacle) yield break;
            
            if (target != null && target is Unit u)
                u.DefendHandler(skill.Unit, (strength - _distance) * skill.Unit.BattleStats.Power, Element.None());

            if (obstacle != null)
                obstacle.DefendHandler(skill.Unit,
                    (strength - _distance) * skill.Unit.BattleStats.Power, Element.None());
        }

        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string str = InfoEffect();
            str +=
                $"\nDamage the Unit {_skillInfo.Unit.BattleStats.Power} x the Number of Cell not crossed if there is an obstacle";
            return str;
        }

        public override string InfoEffect()
        {
            string str = $"<sprite name=Push> {Strength} <sprite name=Direction>";
            if (Strength > 1)
            {
                str += "Cells";
            }
            else str += "Cell";
            str += " Away";
            return str;
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return true;
        }
    }
}