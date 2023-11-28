using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Stats
{
    public enum EElement{None, Fire, Nature, Water}

    [Serializable]
    public struct Affinity
    {
        [FormerlySerializedAs("Fire")]
        public float fire;
        [FormerlySerializedAs("Nature")]
        public float nature;
        [FormerlySerializedAs("Water")]
        public float water;

        public Affinity(float _fire, float _nature, float _water)
        {
            fire = _fire;
            nature = _nature;
            water = _water;
        }

        public Affinity(Affinity _affinity)
        {
            fire = _affinity.fire;
            nature = _affinity.nature;
            water = _affinity.water;
        }

        public Affinity(float _a)
        {
            fire = _a;
            nature = _a;
            water = _a;
        }

        public float GetAffinity(EElement _element)
        {
            switch (_element)
            {
                case EElement.Fire : return fire;
                case EElement.Nature : return nature;
                case EElement.Water : return water;
                default : return 0;
            }
        }

        public static Affinity operator +(Affinity _a, Affinity _b)
        {
            Affinity _ret = new Affinity(_a);
            _ret.fire += _b.fire;
            _ret.nature += _b.nature;
            _ret.water += _b.water;
            return _ret;
        }

        public static Affinity operator +(Affinity _a, int _b)
        {
            Affinity _ret = new Affinity(_a);
            _ret.fire += _b;
            _ret.nature += _b;
            _ret.water += _b;
            return _ret;
        }
        
        public static Affinity operator *(Affinity _a, float _b)
        {
            Affinity _ret = new Affinity(_a);
            _ret.fire *= _b;
            _ret.nature *= _b;
            _ret.water *= _b;
            return _ret;
        }
        
        public static Affinity operator -(Affinity _a, Affinity _b)
        {
            Affinity _ret = new Affinity(_a);
            _ret.fire -= _b.fire;
            _ret.nature -= _b.nature;
            _ret.water -= _b.water;
            return _ret;
        }
        
        public static Affinity operator *(Affinity _a, Affinity _b)
        {
            Affinity _ret = new Affinity(_a);
            _ret.fire *= _b.fire;
            _ret.nature *= _b.nature;
            _ret.water *= _b.water;
            return _ret;
        }

        public static Affinity Pow(Affinity _a, Affinity _b)
        {
            Affinity _ret = new Affinity(_a);
            _ret.fire = Mathf.Pow(_ret.fire,_b.fire);
            _ret.nature = Mathf.Pow(_ret.nature, _b.nature);
            _ret.water = Mathf.Pow(_ret.water, _b.water);
            return _ret;
        }

        public static Affinity Random(Affinity _min, Affinity _max)
        {
            Affinity _ret = new Affinity();
            _ret.fire = UnityEngine.Random.Range(_min.fire, _max.fire);
            _ret.nature = UnityEngine.Random.Range(_min.nature, _max.nature);
            _ret.water = UnityEngine.Random.Range(_min.water, _max.water);
            return _ret;
        }
    }
}