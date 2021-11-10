using System;
using UnityEngine;

namespace Stats
{
    public enum EElement{None, Fire, Nature, Water}

    [Serializable]
    public struct Affinity
    {
        public float Fire;
        public float Nature;
        public float Water;

        public Affinity(float _fire, float _nature, float _water)
        {
            Fire = _fire;
            Nature = _nature;
            Water = _water;
        }

        public Affinity(Affinity _affinity)
        {
            Fire = _affinity.Fire;
            Nature = _affinity.Nature;
            Water = _affinity.Water;
        }

        public Affinity(float a)
        {
            Fire = a;
            Nature = a;
            Water = a;
        }

        public float GetAffinity(EElement Element)
        {
            switch (Element)
            {
                case EElement.Fire : return Fire;
                case EElement.Nature : return Nature;
                case EElement.Water : return Water;
                default : return 0;
            }
        }

        public static Affinity operator +(Affinity a, Affinity b)
        {
            Affinity _ret = new Affinity(a);
            _ret.Fire += b.Fire;
            _ret.Nature += b.Nature;
            _ret.Water += b.Water;
            return _ret;
        }

        public static Affinity operator +(Affinity a, int b)
        {
            Affinity _ret = new Affinity(a);
            _ret.Fire += b;
            _ret.Nature += b;
            _ret.Water += b;
            return _ret;
        }
        
        public static Affinity operator *(Affinity a, float b)
        {
            Affinity _ret = new Affinity(a);
            _ret.Fire *= b;
            _ret.Nature *= b;
            _ret.Water *= b;
            return _ret;
        }
        
        public static Affinity operator -(Affinity a, Affinity b)
        {
            Affinity _ret = new Affinity(a);
            _ret.Fire -= b.Fire;
            _ret.Nature -= b.Nature;
            _ret.Water -= b.Water;
            return _ret;
        }
        
        public static Affinity operator *(Affinity a, Affinity b)
        {
            Affinity _ret = new Affinity(a);
            _ret.Fire *= b.Fire;
            _ret.Nature *= b.Nature;
            _ret.Water *= b.Water;
            return _ret;
        }

        public static Affinity Pow(Affinity a, Affinity b)
        {
            Affinity _ret = new Affinity(a);
            _ret.Fire = Mathf.Pow(_ret.Fire,b.Fire);
            _ret.Nature = Mathf.Pow(_ret.Nature, b.Nature);
            _ret.Water = Mathf.Pow(_ret.Water, b.Water);
            return _ret;
        }

        public static Affinity Random(Affinity _min, Affinity _max)
        {
            Affinity ret = new Affinity();
            ret.Fire = UnityEngine.Random.Range(_min.Fire, _max.Fire);
            ret.Nature = UnityEngine.Random.Range(_min.Nature, _max.Nature);
            ret.Water = UnityEngine.Random.Range(_min.Water, _max.Water);
            return ret;
        }
    }
}