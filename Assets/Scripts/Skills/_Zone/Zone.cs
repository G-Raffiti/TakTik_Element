using System;
using System.Collections.Generic;
using System.Linq;
using Cells;
using GridObjects;
using JetBrains.Annotations;
using StateMachine;
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
        public static List<Cell> GetZone(GridRange _range, Cell _cell)
        {
            switch (_range.zoneType)
            {
                case EZone.Self: return new List<Cell>(1) {_cell};
                case EZone.Basic: return Basic(_cell, _range.radius);
                case EZone.Contact: return Contact(_cell);
                case EZone.Cross: return Cross(_cell, _range.radius);
                case EZone.Ranged: return Ranged(_cell, _range.radius);
                case EZone.Line: return GetLine(_cell, _range.radius);
                case EZone.Cone: return GetCone(_cell, _range.radius);
                case EZone.Perpendicular: return GetPerpendicular(_cell, _range.radius);
                default: return new List<Cell>(1) {_cell};
            }
        }
        
        /// <summary>
        /// Get All the Cell in the Distance of the Range Value with the given Cell as center
        /// </summary>
        /// <param name="range">skill Range</param>
        /// <param name="cell">unit's Cell who play the skill</param>
        /// <returns></returns>
        public static List<Cell> GetRange(GridRange _range, Cell _cell)
        {
            switch (_range.rangeType)
            {
                case EZone.Self: return new List<Cell>() {_cell};
                case EZone.Basic: return Basic(_cell, _range.rangeValue);
                case EZone.Contact: return Contact(_cell);
                case EZone.Cross: return Cross(_cell, _range.rangeValue);
                case EZone.Ranged: return Ranged(_cell, _range.rangeValue);
                default: return new List<Cell>() {_cell};
            }
        }

        
        #region Zone Type

        private static List<Cell> Basic(Cell _cell, int _radius)
        {
            List<Cell> _ret = new List<Cell>();
            _ret.AddRange(BattleStateManager.instance.Cells.Where(_instanceCell => _cell.GetDistance(_instanceCell) <= _radius));

            return _ret;
        }

        private static List<Cell> Contact(Cell _cell)
        {
            return _cell.Neighbours;
        }

        private static List<Cell> Cross(Cell _cell, int _radius)
        {
            List<Cell> _ret = new List<Cell>();
            foreach (Vector2 _direction in Directions)
            {
                for (int _i = 0; _i < _radius + 1; _i++)
                {
                    Cell _cellInRange = BattleStateManager.instance.Cells.Find(_c => _c.OffsetCoord == _cell.OffsetCoord + _direction * _i);
                    if (_cellInRange == null) continue;
                    _ret.Add(_cellInRange);
                }
            }

            return _ret;
        }

        private static List<Cell> Ranged(Cell _cell, int _radius)
        {
            return new List<Cell>(BattleStateManager.instance.Cells.Where(_c => _c.GetDistance(_cell) <= _radius).ToList()).Where(_c2 => _cell.GetDistance(_c2) > 2).ToList();
        }

        public static Vector2 Direction(Cell _start, Cell _destination)
        {
            Vector2 _direction = _start.OffsetCoord - _destination.OffsetCoord;
            if (_direction.x == 0 && _direction.y == 0) 
                return zero;
            if (Math.Abs(_direction.x) > Math.Abs(_direction.y))
                return _direction.x < 0 ? right : left;
            return _direction.y < 0 ? up : down;
        }

        public static Vector2 RectDirection(Vector2 _direction)
        {
            if(_direction == up) return right;
            if(_direction == right) return down;
            if (_direction == down) return left;
            if (_direction == left) return up;
            return zero;
        }
        
        private static Vector2 GetDirection(Cell _targetCell)
        {
            Vector3 _direction = BattleStateManager.instance.PlayingUnit.Cell.OffsetCoord - _targetCell.OffsetCoord;
            if (Math.Abs(Math.Abs(_direction.x) - Math.Abs(_direction.y)) < 0.01f)
            {
                _direction = BattleStateManager.instance.PlayingUnit.Cell.transform
                    .position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Math.Abs(_direction.x) > Math.Abs(_direction.y))
                return _direction.x < 0 ? right : left;
            return _direction.y < 0 ? up : down;
        }

        private static List<Cell> GetLine(Cell _cell, int _radius)
        {
            Vector2 _direction = GetDirection(_cell);
            List<Cell> _ret = new List<Cell>();
            for (int _i = 0; _i < _radius + 1; _i++)
            {
                Cell _cellInRange = BattleStateManager.instance.Cells.Find(_c => _c.OffsetCoord == _cell.OffsetCoord + _direction * _i);
                if (_cellInRange == null) continue;
                if (!_ret.Contains(_cellInRange))
                    _ret.Add(_cellInRange);
            }

            return _ret;
        }

        private static List<Cell> GetCone(Cell _cell, int _radius)
        {
            List<Cell> _ret = new List<Cell>() { };
            Vector2 _direction = GetDirection(_cell);

            List<Cell> _baseLine = new List<Cell>() {_cell};
            for (int _i = 0; _i < _radius + 1; _i++)
            {
                if (BattleStateManager.instance.Cells.Find(_c =>
                    _c.OffsetCoord == _cell.OffsetCoord + _direction * _i + RectDirection(_direction) * _i) != null)
                    _baseLine.Add(BattleStateManager.instance.Cells.Find(_c =>
                        _c.OffsetCoord == _cell.OffsetCoord + _direction * _i + RectDirection(_direction) * _i));
                if (BattleStateManager.instance.Cells.Find(_c =>
                    _c.OffsetCoord == _cell.OffsetCoord + _direction * _i + RectDirection(_direction) * -_i) != null)
                    _baseLine.Add(BattleStateManager.instance.Cells.Find(_c =>
                        _c.OffsetCoord == _cell.OffsetCoord + _direction * _i + RectDirection(_direction) * -_i));
            }

            foreach (Cell _baseCell in _baseLine)
            {
                for (int _i = 0; _i < _radius + 1 - (_baseCell.GetDistance(_cell)) / 2; _i++)
                {
                    Cell _cellInRange =
                        BattleStateManager.instance.Cells.Find(_c =>
                            _c.OffsetCoord == _baseCell.OffsetCoord + _direction * _i);
                    if (_cellInRange == null) continue;
                    if (!_ret.Contains(_cellInRange))
                        _ret.Add(_cellInRange);
                }
            }

            return _ret;
        }

        private static List<Cell> GetPerpendicular(Cell _cell, int _radius)
        {
            List<Cell> _ret = new List<Cell>();
            List<Cell> _line = new List<Cell>();
            if (Math.Abs(GetDirection(_cell).y) > 0)
            {
                _line = new List<Cell>(
                    BattleStateManager.instance.Cells.Where(_c => _c.OffsetCoord.y == _cell.OffsetCoord.y));
            }
            else if (Math.Abs(GetDirection(_cell).x) > 0)
            {
                _line = new List<Cell>(
                    BattleStateManager.instance.Cells.Where(_c => _c.OffsetCoord.x == _cell.OffsetCoord.x));
            }

            _ret = new List<Cell>(_line.Where(_c => _c.GetDistance(_cell) <= _radius));

            return _ret;
        }

        #endregion

        /// <summary>
        /// Get all the Cell on which you can use a skill
        /// </summary>
        /// <returns></returns>
        public static List<Cell> CellsInRange(Skill _skill, Cell _startCell)
        {
            List<Cell> _inRange = new List<Cell>();
            
            foreach (Cell _cell in GetRange(_skill.GridRange, _startCell))
            {
                if (!_skill.GridRange.needTarget && !_inRange.Contains(_cell))
                    _inRange.Add(_cell);
                
                else if (GetAffected(_cell, _skill) != null && !_inRange.Contains(_cell))
                {
                    _inRange.Add(_cell);
                }
            }

            return _inRange;
        }

        /// <summary>
        /// Get all the Cell on which you can use a skill if you need the view
        /// </summary>
        /// <returns></returns>
        public static List<Cell> CellsInView(Skill _skill, Cell _startCell)
        {
            List<Cell> _inRange = new List<Cell>();

            foreach (Cell _cell in FieldOfView(_startCell, GetRange(_skill.GridRange, _startCell)))
            {
                if (!_skill.GridRange.needTarget)
                    _inRange.Add(_cell);
                
                else if (GetAffected(_cell, _skill) != null)
                {
                    _inRange.Add(_cell);
                }
            }

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
            Unit _affected = null;
            switch (_skillInfo.skill.Affect)
            {
                case EAffect.All :
                    if (_cell.IsTaken && _cell.CurrentUnit != null)
                        _affected = _cell.CurrentUnit;
                    break;
                case EAffect.OnlyAlly:
                    if (_cell.IsTaken && _cell.CurrentUnit != null && _cell.CurrentUnit.playerType == _skillInfo.unit.playerType)
                        _affected = _cell.CurrentUnit;
                    break;
                case EAffect.OnlyEnemy:
                    if (_cell.IsTaken && _cell.CurrentUnit != null && _cell.CurrentUnit.playerType != _skillInfo.unit.playerType)
                        _affected = _cell.CurrentUnit;
                    break;
                case EAffect.OnlySelf:
                    if (_cell.IsTaken && _cell.CurrentUnit == _skillInfo.unit)
                        _affected = _cell.CurrentUnit;
                    break;
                case EAffect.OnlyOthers:
                    if (_cell.IsTaken && _cell != _skillInfo.unit.Cell)
                    {
                        _affected = _cell.CurrentUnit;
                    }
                    break;
            }

            return _affected;
        }
        
        /// <summary>
        /// Determine if a Cell is taken by a IMovable who's affected by a Skill and return the IMovable on the Cell if she is
        /// </summary>
        /// <param name="_cell">the cell that have to be tested</param>
        /// <returns></returns>
        [CanBeNull]
        public static Movable GetAffected(Cell _cell, Skill _skill)
        {
            Movable _affected = null;
            switch (_skill.Affect)
            {
                case EAffect.All :
                    if (_cell.IsTaken)
                        _affected = _cell.GetCurrentIMovable();
                    break;
                case EAffect.OnlyAlly:
                    if (_cell.IsTaken && _cell.CurrentUnit != null && _cell.CurrentUnit.playerType == _skill.Unit.playerType)
                        _affected = _cell.CurrentUnit;
                    break;
                case EAffect.OnlyEnemy:
                    if (_cell.IsTaken && _cell.CurrentUnit != null && _cell.CurrentUnit.playerType != _skill.Unit.playerType)
                        _affected = _cell.CurrentUnit;
                    break;
                case EAffect.OnlySelf:
                    if (_cell.IsTaken && _cell.CurrentUnit == _skill.Unit)
                        _affected = _cell.CurrentUnit;
                    break;
                case EAffect.OnlyOthers:
                    if (_cell.IsTaken && _cell != _skill.Unit.Cell)
                    {
                        _affected = _cell.GetCurrentIMovable();
                    }
                    break;
            }

            return _affected;
        }

        public static GridObject GetObjectAffected(Cell _cell, Skill _skill)
        {
            if (GetAffected(_cell, _skill) != null && GetAffected(_cell, _skill) is GridObject _gridObject)
            {
                return _gridObject;
            }
            return null;
        }
        
        public static string AffectToString(EAffect _affect)
        {
            switch (_affect)
            {
                case EAffect.All: return "All Units";
                case EAffect.OnlyAlly: return "Only Ally";
                case EAffect.OnlyEnemy: return "Only Enemy";
                case EAffect.OnlySelf: return "Only You";
                case EAffect.OnlyOthers: return "Only Others";
                default: return _affect.ToString();
            }
        }

        public static string ZoneToString(EZone _type)
        {
            string _str = "";
            switch (_type)
            {
                case EZone.Self:
                    _str += "<sprite name=ZoneSelf>"; break;
                case EZone.Basic:
                    _str += "<sprite name=ZoneBasic>"; break;
                case EZone.Contact:
                    _str += "<sprite name=ZoneContact>"; break;
                case EZone.Line:
                    _str += "<sprite name=ZoneLine>"; break;
                case EZone.Ranged:
                    _str += "<sprite name=ZoneRanged>"; break;
                case EZone.Square:
                    _str += "<sprite name=ZoneSquare>"; break;
                case EZone.Cross:
                    _str += "<sprite name=ZoneCross>"; break;
                case EZone.Perpendicular:
                    _str += "<sprite name=ZonePerpendicular>"; break;
                case EZone.Cone:
                    _str += "<sprite name=ZoneCone>"; break;
                default:
                    _str += _type.ToString(); break;
            }

            return _str;
        }

        public static List<Unit> GetUnitsAffected(SkillInfo _skillInfo, Cell _targetCell)
        {
            List<Unit> _ret = new List<Unit>();
            foreach (Cell _cell in GetZone(_skillInfo.skill.GridRange, _targetCell))
            {
                if (GetUnitAffected(_cell, _skillInfo) != null)
                    _ret.Add(GetUnitAffected(_cell, _skillInfo));
            }

            return _ret;
        }
    }
}