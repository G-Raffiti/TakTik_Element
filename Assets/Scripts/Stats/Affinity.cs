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

        public float GetAffinity(EElement Element)
        {
            switch (Element)
            {
                case EElement.Fire : return Fire / 100f;
                case EElement.Nature : return Nature / 100f;
                case EElement.Water : return Water / 100f;
                default : return 1;
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
            _ret.Fire = ((_ret.Fire/100f) * (b.Fire/100f)) * 100;
            _ret.Nature = ((_ret.Nature/100f) * (b.Nature/100f)) * 100;
            _ret.Water = ((_ret.Water/100f) * (b.Water/100f)) * 100;
            return _ret;
        }

        public static Affinity Pow(Affinity a, Affinity b)
        {
            Affinity _ret = new Affinity(a);
            _ret.Fire = Mathf.Pow((_ret.Fire/100f),(b.Fire/100f)) * 100;
            _ret.Nature = Mathf.Pow((_ret.Nature/100f), (b.Nature/100f)) * 100;
            _ret.Water = Mathf.Pow((_ret.Water/100f), (b.Water/100f)) * 100;
            return _ret;
        }
    }
}