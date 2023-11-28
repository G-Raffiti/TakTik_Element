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

namespace Skills.ScriptableObject_GridEffect
{
    [CreateAssetMenu(fileName = "GridEffect_Bring", menuName = "Scriptable Object/Skills/Grid Effect Bring")]
    public class Pull : SkillGridEffect
    {
        [SerializeField] private int Strength;
        
        public override void Use(Cell _cell, SkillInfo _skillInfo)
        {
            List<Cell> _zone = Zone.GetZone(_skillInfo.skill.GridRange, _cell);
            _zone.Sort((_cell1, _cell2) =>
                _cell1.GetDistance(_skillInfo.Unit.Cell).CompareTo(_cell2.GetDistance(_skillInfo.Unit.Cell)));
            foreach (Cell _cellAffected in _zone)
            {
                IMovable _Affected = Zone.GetAffected(_cellAffected, _skillInfo.skill);
                DataBase.RunCoroutine(PullTo(_skillInfo, _Affected, Strength));
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
        private static IEnumerator PullTo(SkillInfo skill, IMovable target, int strength)
        {
            
            // find the target
            if (target == null) yield break;
            Cell _targetedCell = target.Cell;
            
            // find the closest neighbor of the sender 
            List<Cell> destinations = new List<Cell>(skill.Unit.Cell.Neighbours);
            destinations.Sort((c1, c2) => c1.GetDistance(_targetedCell).CompareTo(c2.GetDistance(_targetedCell)));
            Cell destination = destinations[0];
            
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
            _path = _path.OrderBy(c => _targetedCell.GetDistance(c)).ToList();
            
            // look if there is an Obstacle in the Path
            IMovable obstacle = null;
            List<Cell> deadEnd = _path.Where(c => !c.IsWalkable).ToList();
            if (deadEnd.Count > 0)
            {
                if (deadEnd[0].IsUnderGround)
                    destination = deadEnd[0];
                else
                {
                    obstacle = _path[_path.FindIndex(c => deadEnd[0])].GetCurrentIMovable();
                    destinations = deadEnd[0].Neighbours;
                    destinations.Sort((c1, c2) => c1.GetDistance(_targetedCell).CompareTo(c2.GetDistance(_targetedCell)));
                    destination = destinations[0];
                }
            }
            
            // determine how much Cell the target should be Pulled and determine the final Path
            if (_targetedCell.GetDistance(destination) > strength)
            {
                destination = _path[strength - 1];
            }
            _path = new List<Cell>(_paths[destination]);

            // Move
            target.Move(destination, _path);
            while (!target.IsMoving) yield return null;
            
            // If an Unit hit an other object, both take damage
            if (_path.Count < strength) 
            {
                if (target is Unit u)
                    u.DefendHandler(skill.Unit, (strength - _path.Count) * skill.Unit.BattleStats.Power, Element.None());
                if (obstacle is Unit o)
                    o.DefendHandler(skill.Unit, (strength - _path.Count) * skill.Unit.BattleStats.Power, Element.None());
            }
        }

        public override string InfoEffect()
        {
            string str = $"Bring All Targets {Strength} ";
            if (Strength > 1)
            {
                str += "Cells";
            }
            else str += "Cell";

            str += " to you or in the direction of the targeted Cell";
            
            return str;
        }
        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string str = $"Bring All Targets {Strength} ";
            if (Strength > 1)
            {
                str += "Cells";
            }
            else str += "Cell";

            if (_skillInfo.skill.GridRange.Radius == 0 || _skillInfo.skill.GridRange.ZoneType == EZone.Self)
            {
                str += " to you";
            }
            else str += " in the direction of the targeted Cell";
            
            return str;
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return true;
        }
    }
}