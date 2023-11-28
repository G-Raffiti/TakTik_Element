using System;
using Skills._Zone;
using Units;
using Random = UnityEngine.Random;

namespace Stats
{
    [Serializable]
    public struct GridRange
    {
        public EZone RangeType;
        public int RangeValue;
        public EZone ZoneType;
        public int Radius;
        public bool NeedView;
        public bool NeedTarget;
        public bool CanBeModified;

        public GridRange(EZone _rangeType, EZone _zoneType, float range, float radius)
        {
            RangeType = _rangeType;
            RangeValue = (int) range;
            ZoneType = _zoneType;
            Radius = (int) radius;
            NeedView = true;
            NeedTarget = false;
            CanBeModified = true;
        }
        public GridRange(GridRange _grid_range)
        {
            RangeType = _grid_range.RangeType;
            RangeValue = _grid_range.RangeValue;
            ZoneType = _grid_range.ZoneType;
            Radius = _grid_range.Radius;
            NeedView = _grid_range.NeedView;
            NeedTarget = _grid_range.NeedTarget;
            CanBeModified = _grid_range.CanBeModified;
        }

        public GridRange(float a)
        {
            RangeType = EZone.Basic;
            RangeValue = (int)a;
            ZoneType = EZone.Basic;
            Radius = (int)a;
            NeedView = true;
            NeedTarget = true;
            CanBeModified = true;
        }

        public static GridRange operator +(GridRange a, GridRange b)
        {
            GridRange _ret = new GridRange(a);
            _ret.RangeValue += b.RangeValue;
            _ret.Radius += b.Radius;
            return _ret;
        }
        
        public static GridRange operator +(GridRange a, float b)
        {
            GridRange _ret = new GridRange(a);
            _ret.RangeValue = (int)(_ret.RangeValue * b);
            _ret.Radius = (int)(_ret.Radius * b);
            return _ret;
        }
        
        public static GridRange operator -(GridRange a, GridRange b)
        {
            GridRange _ret = new GridRange(a);
            _ret.RangeValue -= b.RangeValue;
            _ret.Radius -= b.Radius;
            return _ret;
        }
        public static GridRange operator -(GridRange a, float b)
        {
            GridRange _ret = new GridRange(a);
            _ret.RangeValue = (int)(_ret.RangeValue * b);
            _ret.Radius = (int)(_ret.Radius * b);
            return _ret;
        }
        public static GridRange operator *(GridRange a, GridRange b)
        {
            GridRange _ret = new GridRange(a);
            _ret.RangeValue *= b.RangeValue;
            _ret.Radius *= b.Radius;
            return _ret;
        }
        public static GridRange operator *(GridRange a, float b)
        {
            GridRange _ret = new GridRange(a);
            _ret.RangeValue = (int)(_ret.RangeValue * b);
            _ret.Radius = (int)(_ret.Radius * b);
            return _ret;
        }

        public static GridRange Randomize(GridRange min, GridRange max)
        {
            GridRange ret = new GridRange();
            
            int r = Random.Range(0, 2);
            ret.RangeType = min.RangeType;
            if (r > 0) ret.RangeType = max.RangeType;

            ret.RangeValue = Random.Range(min.RangeValue, max.RangeValue);

            int z = Random.Range(0, 2);
            ret.ZoneType = min.ZoneType;
            if (z > 0) ret.ZoneType = max.ZoneType;

            ret.Radius = Random.Range(min.Radius, max.Radius);

            int nv = Random.Range(0, 2);
            ret.NeedView = min.NeedView;
            if (nv > 0) ret.NeedView = max.NeedView;

            int nt = Random.Range(0, 2);
            ret.NeedTarget = min.NeedTarget;
            if (nt > 0) ret.NeedTarget = max.NeedTarget;
            
            int sc = Random.Range(0, 2);

            int cm = Random.Range(0, 2);
            ret.CanBeModified = min.CanBeModified;
            if (cm > 0) ret.CanBeModified = max.CanBeModified;

            return ret;
        }
        
        public override string ToString()
        {
            string str = $"Range: {RangeValue} {Zone.ZoneToString(RangeType)}\n";
            if (Radius > 0)
            {
                str += $"Zone: {Radius} {Zone.ZoneToString(ZoneType)}\n";
            }

            if (NeedTarget || !NeedView) str += "<size=35>";
            if (!NeedView) str += "<sprite name=DontNeedView>";
            if (NeedTarget) str += "<sprite name=NeedTarget>";
            if (!CanBeModified) str += "\n<size=25>Range Can't be modified\n";
            return str;
        }

        public string toStringForUnit()
        {
            return $"<sprite name=Range>Range <color=orange>{RangeValue}</color> {Zone.ZoneToString(RangeType)}\n<sprite name=Zone>Zone <color=orange>{Radius}</color> {Zone.ZoneToString(ZoneType)}";
        }
    }
}