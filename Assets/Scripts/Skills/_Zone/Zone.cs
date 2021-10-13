using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cells;
using Grid;
using GridObjects;
using Stats;
using Units;
using UnityEngine;
using static UnityEngine.Vector2;

namespace Skills._Zone
{
    public enum EZone
    {
        Self, 
        Basic, 
        Contact, 
        Line, 
        Ranged ,
        Square, 
        Cross, 
        Cone, 
        Perpendicular,
    }

    public enum EAffect
    {
        All, 
        OnlyAlly, 
        OnlyEnemy, 
        OnlySelf, 
        OnlyOthers,
        OnlyUnits,
    }

    public static class Zone
    {
        
        private static readonly Vector2[] Directions =
        {
            new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1)
        };
        
        
        /// <summary>
        /// Get All Cell Affected by the Zone of the Range with the given Cell as center
        /// </summary>
        /// <param name="range">skill Range</param>
        /// <param name="cell">Targeted Cell</param>
        /// <returns></returns>
        public static List<Cell> GetZone(Range range, Cell cell)
        {
            switch (range.ZoneType)
            {
                case EZone.Self: return new List<Cell>(1) {cell};
                case EZone.Basic: return Basic(cell, range.Radius);
                case EZone.Contact: return Contact(cell);
                case EZone.Cross: return Cross(cell, range.Radius);
                case EZone.Ranged: return Ranged(cell, range.Radius);
                case EZone.Line: return GetLine(cell, range.Radius);
                case EZone.Cone: return GetCone(cell, range.Radius);
                case EZone.Perpendicular: return GetPerpendicular(cell, range.Radius);
                default: return new List<Cell>(1) {cell};
            }
        }
        
        /// <summary>
        /// Get All the Cell in the Distance of the Range Value with the given Cell as center
        /// </summary>
        /// <param name="range">skill Range</param>
        /// <param name="cell">unit's Cell who play the skill</param>
        /// <returns></returns>
        public static List<Cell> GetRange(Range range, Cell cell)
        {
            switch (range.RangeType)
            {
                case EZone.Self: return new List<Cell>() {cell};
                case EZone.Basic: return Basic(cell, range.RangeValue);
                case EZone.Contact: return Contact(cell);
                case EZone.Cross: return Cross(cell, range.RangeValue);
                case EZone.Ranged: return Ranged(cell, range.RangeValue);
                default: return new List<Cell>() {cell};
            }
        }

        
        #region Zone Type

        private static List<Cell> Basic(Cell cell, int radius)
        {
            List<Cell> _ret = new List<Cell>();
            _ret.AddRange(BattleStateManager.instance.Cells.Where(_instanceCell => cell.GetDistance(_instanceCell) <= radius));

            return _ret;
        }

        private static List<Cell> Contact(Cell cell)
        {
            return cell.Neighbours;
        }

        private static List<Cell> Cross(Cell cell, int radius)
        {
            List<Cell> _ret = new List<Cell>();
            foreach (Vector2 _direction in Directions)
            {
                for (int i = 0; i < radius + 1; i++)
                {
                    Cell _cellInRange = BattleStateManager.instance.Cells.Find(c => c.OffsetCoord == cell.OffsetCoord + _direction * i);
                    if (_cellInRange == null) continue;
                    _ret.Add(_cellInRange);
                }
            }

            return _ret;
        }

        private static List<Cell> Ranged(Cell cell, int radius)
        {
            return new List<Cell>(BattleStateManager.instance.Cells.Where(c => c.GetDistance(cell) <= radius).ToList()).Where(_cell => _cell.GetDistance(cell) > 2).ToList();
        }

        public static Vector2 Direction(Cell start, Cell destination)
        {
            Vector2 direction = start.OffsetCoord - destination.OffsetCoord;
            if (direction.x == 0 && direction.y == 0) 
                return zero;
            if (Math.Abs(direction.x) > Math.Abs(direction.y))
                return direction.x < 0 ? right : left;
            return direction.y < 0 ? up : down;
        }

        public static Vector2 RectDirection(Vector2 direction)
        {
            if(direction == up) return right;
            if(direction == right) return down;
            if (direction == down) return left;
            if (direction == left) return up;
            return zero;
        }
        
        private static Vector2 GetDirection(Cell targetCell)
        {
            Vector3 direction = BattleStateManager.instance.PlayingUnit.Cell.OffsetCoord - targetCell.OffsetCoord;
            if (Math.Abs(Math.Abs(direction.x) - Math.Abs(direction.y)) < 0.01f)
            {
                direction = BattleStateManager.instance.PlayingUnit.Cell.transform
                    .position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Math.Abs(direction.x) > Math.Abs(direction.y))
                return direction.x < 0 ? right : left;
            return direction.y < 0 ? up : down;
        }

        private static List<Cell> GetLine(Cell cell, int radius)
        {
            Vector2 direction = GetDirection(cell);
            List<Cell> _ret = new List<Cell>();
            for (int i = 0; i < radius + 1; i++)
            {
                Cell _cellInRange = BattleStateManager.instance.Cells.Find(c => c.OffsetCoord == cell.OffsetCoord + direction * i);
                if (_cellInRange == null) continue;
                _ret.Add(_cellInRange);
            }

            return _ret;
        }

        private static List<Cell> GetCone(Cell cell, int radius)
        {
            List<Cell> _ret = new List<Cell>() { };
            Vector2 direction = GetDirection(cell);

            List<Cell> _baseLine = new List<Cell>() {cell};
            for (int i = 0; i < radius + 1; i++)
            {
                if (BattleStateManager.instance.Cells.Find(c =>
                    c.OffsetCoord == cell.OffsetCoord + direction * i + RectDirection(direction) * i) != null)
                    _baseLine.Add(BattleStateManager.instance.Cells.Find(c =>
                        c.OffsetCoord == cell.OffsetCoord + direction * i + RectDirection(direction) * i));
                if (BattleStateManager.instance.Cells.Find(c =>
                    c.OffsetCoord == cell.OffsetCoord + direction * i + RectDirection(direction) * -i) != null)
                    _baseLine.Add(BattleStateManager.instance.Cells.Find(c =>
                        c.OffsetCoord == cell.OffsetCoord + direction * i + RectDirection(direction) * -i));
            }

            foreach (Cell baseCell in _baseLine)
            {
                for (int i = 0; i < radius + 1 - (baseCell.GetDistance(cell)) / 2; i++)
                {
                    Cell _cellInRange =
                        BattleStateManager.instance.Cells.Find(c =>
                            c.OffsetCoord == baseCell.OffsetCoord + direction * i);
                    if (_cellInRange == null) continue;
                    _ret.Add(_cellInRange);
                }
            }

            return _ret;
        }

        private static List<Cell> GetPerpendicular(Cell cell, int radius)
        {
            List<Cell> _ret = new List<Cell>();
            List<Cell> _line = new List<Cell>();
            if (Math.Abs(GetDirection(cell).y) > 0)
            {
                _line = new List<Cell>(
                    BattleStateManager.instance.Cells.Where(c => c.OffsetCoord.y == cell.OffsetCoord.y));
            }
            else if (Math.Abs(GetDirection(cell).x) > 0)
            {
                _line = new List<Cell>(
                    BattleStateManager.instance.Cells.Where(c => c.OffsetCoord.x == cell.OffsetCoord.x));
            }

            _ret = new List<Cell>(_line.Where(c => c.GetDistance(cell) <= radius));

            return _ret;
        }

        #endregion

        /// <summary>
        /// Get all the Cell on which you can use a skill
        /// </summary>
        /// <returns></returns>
        public static List<Cell> CellsInRange(SkillInfo skill)
        {
            Cell _startCell = skill.Unit.Cell;
            List<Cell> _inRange = new List<Cell>();
            
            foreach (Cell _cell in GetRange(skill.Range, _startCell))
            {
                if (!skill.Range.NeedTarget)
                    _inRange.Add(_cell);
                
                else if (GetAffected(_cell, skill) != null)
                {
                    _inRange.Add(_cell);
                }
            }
            
            if (!skill.Range.SelfCast)
                _inRange.Remove(_startCell);

            return _inRange;
        }

        /// <summary>
        /// Get all the Cell on which you can use a skill if you need the view
        /// </summary>
        /// <returns></returns>
        public static List<Cell> CellsInView(SkillInfo skill)
        {
            Cell _startCell = skill.Unit.Cell;
            List<Cell> _inRange = new List<Cell>();

            foreach (Cell _cell in FieldOfView(_startCell, GetRange(skill.Range, _startCell)))
            {
                if (!skill.Range.NeedTarget)
                    _inRange.Add(_cell);
                
                else if (GetAffected(_cell, skill) != null)
                {
                    _inRange.Add(_cell);
                }
            }
            
            if (!skill.Range.SelfCast)
                _inRange.Remove(_startCell);

            return _inRange;
        }

        /// <summary>
        /// Methode to determine if you can see the Cells from a List of Cell (return all Cell that you can see)
        /// </summary>
        /// <param name="_startCell">from where you are looking</param>
        /// <param name="_cellsInRange">The List of cell you want to test (if you want to try every Cell use BattleStateManager.instance.Cells) </param>
        /// <returns></returns>
        private static List<Cell> FieldOfView(Cell _startCell, List<Cell> _cellsInRange)
        {
            List<Cell> _fieldOfView = new List<Cell>();
            List<Cell> _notTested = new List<Cell>(_cellsInRange);
            Vector3 _start = _startCell.transform.position;
            foreach (Cell _cell in _notTested)
            {
                Vector3 _position = _cell.transform.position;
                
                RaycastHit2D[] _hits = Physics2D.LinecastAll(_start, _position);
                
                List<Cell> _inTest = (from _hit in _hits where _hit.collider.GetComponent<Cell>() != null select _hit.collider.gameObject.GetComponent<Cell>()).ToList();

                foreach (Cell _t in _inTest)
                {
                    if (!_cellsInRange.Contains(_t))
                        continue;
                    if (!_fieldOfView.Contains(_t))
                    {
                        if (!_t.IsTaken)
                            _fieldOfView.Add(_t);
                        else if (_t.GetCurrentIMovable() != null)
                            _fieldOfView.Add(_t);
                    }
                    
                    if (_t.IsTaken && !_t.Equals(_startCell))
                        break;
                }
            }

            return _fieldOfView;
        }
        
        /// <summary>
        /// Determine if a Cell is taken by an Unit who's affected by a Skill and return the Unit on the Cell if she is
        /// </summary>
        /// <param name="_cell">the cell that have to be tested</param>
        /// <returns></returns>
        public static Unit GetUnitAffected(Cell _cell, SkillInfo _skillInfo)
        {
            if (GetAffected(_cell, _skillInfo) is Unit _affected)
                return _affected;
            return null;
        }
        
        /// <summary>
        /// Determine if a Cell is taken by a IMovable who's affected by a Skill and return the IMovable on the Cell if she is
        /// </summary>
        /// <param name="_cell">the cell that have to be tested</param>
        /// <returns></returns>
        public static IMovable GetAffected(Cell _cell, SkillInfo _skillInfo)
        {
            IMovable _affected = null;
            switch (_skillInfo.Affect)
            {
                case EAffect.All :
                        _affected = _cell.GetCurrentIMovable();
                    break;
                case EAffect.OnlyAlly:
                    if (_cell.IsTaken && _cell.CurrentUnit != null && _cell.CurrentUnit.playerNumber == _skillInfo.Unit.playerNumber)
                        _affected = _cell.CurrentUnit;
                    break;
                case EAffect.OnlyEnemy:
                    if (_cell.IsTaken && _cell.CurrentUnit != null && _cell.CurrentUnit.playerNumber != _skillInfo.Unit.playerNumber)
                        _affected = _cell.CurrentUnit;
                    break;
                case EAffect.OnlySelf:
                    if (_cell.IsTaken && _cell.CurrentUnit == _skillInfo.Unit)
                        _affected = _cell.CurrentUnit;
                    break;
                case EAffect.OnlyOthers:
                    if (_cell != _skillInfo.Unit.Cell)
                    {
                        _affected = _cell.GetCurrentIMovable();
                    }
                    break;
                default: 
                    _affected = _cell.GetCurrentIMovable();
                    break;
            }

            return _affected;
        }

        public static GridObject GetObjectAffected(Cell _cell, SkillInfo _skillInfo)
        {
            if (GetAffected(_cell, _skillInfo) is GridObject _gridObject)
            {
                return _gridObject;
            }
            return null;
        }
        
        public static string AffectToString(EAffect affect)
        {
            switch (affect)
            {
                case EAffect.All: return "All Units";
                case EAffect.OnlyAlly: return "Only Ally";
                case EAffect.OnlyEnemy: return "Only Enemy";
                case EAffect.OnlySelf: return "Only You";
                case EAffect.OnlyOthers: return "Only Others";
                default: return affect.ToString();
            }
        }

        public static string ZoneToString(EZone type)
        {
            switch (type)
            {
                case EZone.Self:
                    return "<sprite name=ZoneSelf>";
                case EZone.Basic:
                    return "<sprite name=ZoneBasic>";
                case EZone.Contact:
                    return "<sprite name=ZoneContact>";
                case EZone.Line:
                    return "<sprite name=ZoneLine>";
                case EZone.Ranged:
                    return "<sprite name=ZoneRanged>";
                case EZone.Square:
                    return "<sprite name=ZoneSquare>";
                case EZone.Cross:
                    return "<sprite name=ZoneCross>";
                default:
                    return type.ToString();
            }
        }
    }
}