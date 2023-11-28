using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Instances;
using _Pathfinding.Algorithms;
using _ScriptableObject;
using Cells;
using Skills._Zone;
using StateMachine;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Skills.ScriptableObject_GridEffect
{
    [CreateAssetMenu(fileName = "GridEffect_Push", menuName = "Scriptable Object/Skills/Grid Effect Push Fix")]
    public class PushFix : SkillGridEffect
    {
        [FormerlySerializedAs("Strength")]
        [SerializeField] private int strength;
        
        public override void Use(Cell _targetCell, SkillInfo _skillInfo)
        {
            List<Cell> _zone = Zone.GetZone(_skillInfo.skill.GridRange, _targetCell);

            List<Movable> _affecteds = new List<Movable>();
            foreach (Cell _cellAffected in _zone)
            {
                if (Zone.GetAffected(_cellAffected, _skillInfo.skill) != null)
                    _affecteds.Add(Zone.GetAffected(_cellAffected, _skillInfo.skill));
            }

            if (_affecteds.Count == 0) return;
            
            Utility.RunCoroutine(Push(_affecteds, _skillInfo.unit.Cell, _skillInfo, strength));
        }

        /// <summary>
        /// Coroutine that move 
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="_from"></param>
        /// <param name="skill"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public static IEnumerator Push(List<Movable> _targets, Cell _from, SkillInfo _skill, int _strength)
        {
            _targets.Sort((_target1, _target2) =>
                _target1.Cell.GetDistance(_from).CompareTo(_target2.Cell.GetDistance(_from)));
            _targets.Reverse();
            foreach (Movable _target in _targets)
            {
                Utility.RunCoroutine(Push(_from, _skill, _target, _strength));
                while (_target.IsMoving)
                {
                    yield return null;
                }
            }
        }

        public static IEnumerator Push(Cell _from, SkillInfo _skill, Movable _target, int _strength)
        {
            // find the target
            if (_target == null) yield break;
            Cell _targetedCell = _target.Cell;
            _target.IsMoving = true;

            // find the destination
            Cell _destination = _target.Cell;

            for (int _i = 1; _i <= _strength; _i++)
            {
                Cell _arrival = BattleStateManager.instance.Cells.Find(_c =>
                    _c.OffsetCoord == _target.Cell.OffsetCoord + Zone.Direction(_from, _targetedCell) * _i);
                if (_arrival == null) break;
                _destination = _arrival;
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
            DijkstraPathfinding _pathfinder = new DijkstraPathfinding();
            Dictionary<Cell, List<Cell>> _paths = _pathfinder.FindAllPaths(_edges, _targetedCell);
            List<Cell> _path = _paths[_destination];
            _path = _path.OrderBy(_c => _targetedCell.GetDistance(_c)).Reverse().ToList();

            // Move
            int _distance = _target.Move(_destination, _path).Count;
            if(_distance != 0)
                while (_target.IsMoving) yield return null;
            
            Debug.Log($"Push : {_skill.unit.unitName} pushed {_target.GetName} of {_distance} Cells");
            
            // If an Unit hit an other object, both take damage
            if (_distance >= _strength) yield break;
            Cell _obstacleCell = BattleStateManager.instance.Cells.Find(_c =>
                _c.OffsetCoord == _target.Cell.OffsetCoord + Zone.Direction(_from, _targetedCell));
            Unit _obstacle = null;
            
            if (_obstacleCell != null)
                 _obstacle = _obstacleCell.CurrentUnit;
            
            if (_target == _obstacle) yield break;
            
            if (_target != null && _target is Unit _u)
                _u.DefendHandler(_skill.unit, (_strength - _distance) * _skill.unit.battleStats.power, Element.None());

            if (_obstacle != null)
                _obstacle.DefendHandler(_skill.unit,
                    (_strength - _distance) * _skill.unit.battleStats.power, Element.None());
        }

        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string _str = InfoEffect();
            _str +=
                $"\nDamage the Unit {_skillInfo.unit.battleStats.power} x the Number of Cell not crossed if there is an obstacle";
            return _str;
        }

        public override string InfoEffect()
        {
            string _str = $"<sprite name=Push> {strength} <sprite name=Direction>";
            if (strength > 1)
            {
                _str += "Cells";
            }
            else _str += "Cell";
            _str += " Away";
            return _str;
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return true;
        }
    }
}