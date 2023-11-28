using System;
using Skills._Zone;
using Units;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Stats
{
    [Serializable]
    public struct GridRange
    {
        [FormerlySerializedAs("RangeType")]
        public EZone rangeType;
        [FormerlySerializedAs("RangeValue")]
        public int rangeValue;
        [FormerlySerializedAs("ZoneType")]
        public EZone zoneType;
        [FormerlySerializedAs("Radius")]
        public int radius;
        [FormerlySerializedAs("NeedView")]
        public bool needView;
        [FormerlySerializedAs("NeedTarget")]
        public bool needTarget;
        [FormerlySerializedAs("CanBeModified")]
        public bool canBeModified;

        public GridRange(EZone _rangeType, EZone _zoneType, float _range, float _radius)
        {
            rangeType = _rangeType;
            rangeValue = (int) _range;
            zoneType = _zoneType;
            this.radius = (int) _radius;
            needView = true;
            needTarget = false;
            canBeModified = true;
        }
        public GridRange(GridRange _gridRange)
        {
            rangeType = _gridRange.rangeType;
            rangeValue = _gridRange.rangeValue;
            zoneType = _gridRange.zoneType;
            radius = _gridRange.radius;
            needView = _gridRange.needView;
            needTarget = _gridRange.needTarget;
            canBeModified = _gridRange.canBeModified;
        }

        public GridRange(float _a)
        {
            rangeType = EZone.Basic;
            rangeValue = (int)_a;
            zoneType = EZone.Basic;
            radius = (int)_a;
            needView = true;
            needTarget = true;
            canBeModified = true;
        }

        public static GridRange operator +(GridRange _a, GridRange _b)
        {
            GridRange _ret = new GridRange(_a);
            _ret.rangeValue += _b.rangeValue;
            _ret.radius += _b.radius;
            return _ret;
        }
        
        public static GridRange operator +(GridRange _a, float _b)
        {
            GridRange _ret = new GridRange(_a);
            _ret.rangeValue = (int)(_ret.rangeValue * _b);
            _ret.radius = (int)(_ret.radius * _b);
            return _ret;
        }
        
        public static GridRange operator -(GridRange _a, GridRange _b)
        {
            GridRange _ret = new GridRange(_a);
            _ret.rangeValue -= _b.rangeValue;
            _ret.radius -= _b.radius;
            return _ret;
        }
        public static GridRange operator -(GridRange _a, float _b)
        {
            GridRange _ret = new GridRange(_a);
            _ret.rangeValue = (int)(_ret.rangeValue * _b);
            _ret.radius = (int)(_ret.radius * _b);
            return _ret;
        }
        public static GridRange operator *(GridRange _a, GridRange _b)
        {
            GridRange _ret = new GridRange(_a);
            _ret.rangeValue *= _b.rangeValue;
            _ret.radius *= _b.radius;
            return _ret;
        }
        public static GridRange operator *(GridRange _a, float _b)
        {
            GridRange _ret = new GridRange(_a);
            _ret.rangeValue = (int)(_ret.rangeValue * _b);
            _ret.radius = (int)(_ret.radius * _b);
            return _ret;
        }

        public static GridRange Randomize(GridRange _min, GridRange _max)
        {
            GridRange _ret = new GridRange();
            
            int _r = Random.Range(0, 2);
            _ret.rangeType = _min.rangeType;
            if (_r > 0) _ret.rangeType = _max.rangeType;

            _ret.rangeValue = Random.Range(_min.rangeValue, _max.rangeValue);

            int _z = Random.Range(0, 2);
            _ret.zoneType = _min.zoneType;
            if (_z > 0) _ret.zoneType = _max.zoneType;

            _ret.radius = Random.Range(_min.radius, _max.radius);

            int _nv = Random.Range(0, 2);
            _ret.needView = _min.needView;
            if (_nv > 0) _ret.needView = _max.needView;

            int _nt = Random.Range(0, 2);
            _ret.needTarget = _min.needTarget;
            if (_nt > 0) _ret.needTarget = _max.needTarget;
            
            int _sc = Random.Range(0, 2);

            int _cm = Random.Range(0, 2);
            _ret.canBeModified = _min.canBeModified;
            if (_cm > 0) _ret.canBeModified = _max.canBeModified;

            return _ret;
        }
        
        public override string ToString()
        {
            string _str = $"Range: {rangeValue} {Zone.ZoneToString(rangeType)}\n";
            if (radius > 0)
            {
                _str += $"Zone: {radius} {Zone.ZoneToString(zoneType)}\n";
            }

            if (needTarget || !needView) _str += "<size=35>";
            if (!needView) _str += "<sprite name=DontNeedView>";
            if (needTarget) _str += "<sprite name=NeedTarget>";
            if (!canBeModified) _str += "\n<size=25>Range Can't be modified\n";
            return _str;
        }

        public string ToStringForUnit()
        {
            return $"<sprite name=Range>Range <color=orange>{rangeValue}</color> {Zone.ZoneToString(rangeType)}\n<sprite name=Zone>Zone <color=orange>{radius}</color> {Zone.ZoneToString(zoneType)}";
        }
    }
}