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
    [CreateAssetMenu(fileName = "GridEffect_Bring", menuName = "Scriptable Object/Skills/Grid Effect Bring")]
    public class Pull : SkillGridEffect
    {
        [FormerlySerializedAs("Strength")]
        [SerializeField] private int strength;
        
        public override void Use(Cell _targetCell, SkillInfo _skillInfo)
        {
            List<Cell> _zone = Zone.GetZone(_skillInfo.skill.GridRange, _targetCell);
            _zone.Sort((_cell1, _cell2) =>
                _cell1.GetDistance(_skillInfo.unit.Cell).CompareTo(_cell2.GetDistance(_skillInfo.unit.Cell)));
            foreach (Cell _cellAffected in _zone)
            {
                Movable _affected = Zone.GetAffected(_cellAffected, _skillInfo.skill);
                Utility.RunCoroutine(PullTo(_skillInfo, _affected, strength));
            }
        }

        /// <summary>
        /// Coroutine that Pull a Target in the direction of the Skill User.
        /// </summary>
        /// <param name="skill">
        /// Skill Info that is used to trigger the Pull
        /// </param>
        /// <param name="target">
        /// IMovable that will move
        /// </param>
        /// <param name="strength">
        /// Distance of Pulling
        /// </param>
        /// <returns></returns>
        private static IEnumerator PullTo(SkillInfo _skill, Movable _target, int _strength)
        {
            
            // find the target
            if (_target == null) yield break;
            Cell _targetedCell = _target.Cell;
            
            // find the closest neighbor of the sender 
            List<Cell> _destinations = new List<Cell>(_skill.unit.Cell.Neighbours);
            _destinations.Sort((_c1, _c2) => _c1.GetDistance(_targetedCell).CompareTo(_c2.GetDistance(_targetedCell)));
            Cell _destination = _destinations[0];
            
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
            _path = _path.OrderBy(_c => _targetedCell.GetDistance(_c)).ToList();
            
            // look if there is an Obstacle in the Path
            Movable _obstacle = null;
            List<Cell> _deadEnd = _path.Where(_c => !_c.IsWalkable).ToList();
            if (_deadEnd.Count > 0)
            {
                if (_deadEnd[0].IsUnderGround)
                    _destination = _deadEnd[0];
                else
                {
                    _obstacle = _path[_path.FindIndex(_c => _deadEnd[0])].GetCurrentIMovable();
                    _destinations = _deadEnd[0].Neighbours;
                    _destinations.Sort((_c1, _c2) => _c1.GetDistance(_targetedCell).CompareTo(_c2.GetDistance(_targetedCell)));
                    _destination = _destinations[0];
                }
            }
            
            // determine how much Cell the target should be Pulled and determine the final Path
            if (_targetedCell.GetDistance(_destination) > _strength)
            {
                _destination = _path[_strength - 1];
            }
            _path = new List<Cell>(_paths[_destination]);

            // Move
            _target.Move(_destination, _path);
            while (!_target.IsMoving) yield return null;
            
            // If an Unit hit an other object, both take damage
            if (_path.Count < _strength) 
            {
                if (_target is Unit _u)
                    _u.DefendHandler(_skill.unit, (_strength - _path.Count) * _skill.unit.battleStats.power, Element.None());
                if (_obstacle is Unit _o)
                    _o.DefendHandler(_skill.unit, (_strength - _path.Count) * _skill.unit.battleStats.power, Element.None());
            }
        }

        public override string InfoEffect()
        {
            string _str = $"Bring All Targets {strength} ";
            if (strength > 1)
            {
                _str += "Cells";
            }
            else _str += "Cell";

            _str += " to you or in the direction of the targeted Cell";
            
            return _str;
        }
        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string _str = $"Bring All Targets {strength} ";
            if (strength > 1)
            {
                _str += "Cells";
            }
            else _str += "Cell";

            if (_skillInfo.skill.GridRange.radius == 0 || _skillInfo.skill.GridRange.zoneType == EZone.Self)
            {
                _str += " to you";
            }
            else _str += " in the direction of the targeted Cell";
            
            return _str;
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return true;
        }
    }
}