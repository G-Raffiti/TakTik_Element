using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Stats
{
    [Serializable]
    public class Power
    {
        [SerializeField] public int Basic;
        [SerializeField] public float Spell = 100;
        [SerializeField] public float Skill = 100;
        [SerializeField] public Affinity Affinity = new Affinity(100, 100, 100);

        public int Magic(EElement Element) => (int) (Basic * (Spell / 100f) * Affinity.GetAffinity(Element));
        public int MagicPercent(EElement ele) => (int) ((Spell / 100f) * Affinity.GetAffinity(ele) * 100 - 100);
        public int Physic(EElement Element) => (int) (Basic * (Skill / 100f) * Affinity.GetAffinity(Element));
        public int PhysicPercent(EElement ele) => (int) ((Skill / 100f) * Affinity.GetAffinity(ele) * 100 - 100);
        
        public Power()
        {
            Basic = 0;
            Spell = 100;
            Skill = 100;
            Affinity = new Affinity(100, 100, 100);
        }
        
        public Power(int _basic)
        {
            Basic = _basic;
            Spell = 100;
            Skill = 100;
            Affinity = new Affinity(100, 100, 100);
        }

        public Power(Power _power)
        {
            Basic = _power.Basic;
            Spell = _power.Spell;
            Skill = _power.Skill;
            Affinity = _power.Affinity;
        }

        public Power(int basic, float spell, float skill, Affinity affinity, int focus)
        {
            Basic = basic;
            Skill = skill;
            Spell = spell;
            Affinity = affinity;
        }

        public static Power operator +(Power a, Power b)
        {
            Power _ret = new Power(a);
            _ret.Basic += b.Basic;
            _ret.Spell = ((_ret.Spell / 100f) * (b.Spell / 100f)) * 100;
            _ret.Skill = ((_ret.Skill / 100f) * (b.Skill / 100f)) * 100;
            _ret.Affinity *= b.Affinity;

            return _ret;
        }

        public static Power operator -(Power a, Power b)
        {
            Power _ret = new Power(a);
            _ret.Basic -= b.Basic;
            _ret.Spell = ((_ret.Spell / 100f) * (100 - b.Spell) / 100f) * 100;
            _ret.Skill = ((_ret.Skill / 100f) * (100 - b.Skill) / 100f) * 100;
            _ret.Affinity *= b.Affinity;

            return _ret;
        }
        public static Power operator *(Power a, Power b)
        {
            Power _ret = new Power(a);
            _ret.Basic *= b.Basic;
            _ret.Spell = Mathf.Pow((_ret.Spell / 100f),(b.Spell / 100f))*100;
            _ret.Skill = Mathf.Pow((_ret.Skill / 100f),(b.Skill / 100f))*100;
            _ret.Affinity = Affinity.Pow(_ret.Affinity, b.Affinity);

            return _ret;
        }

        /// <summary>
        /// Add an Integer value to the Basic value of power
        /// </summary>
        public static Power operator +(Power power, int basic)
        {
            Power _ret = new Power(power);
            _ret.Basic += basic;
            return _ret;
        }
        
        /// <summary>
        /// Multiple the Basic value of power by a float
        /// </summary>
        public static Power operator *(Power power, float basic)
        {
            Power _ret = new Power(power);
            _ret.Basic = (int) (_ret.Basic * basic);
            return _ret;
        }
        
        public void AddSpell(float spell)
        {
            Spell += spell;
        }
        
        public void AddSkill(float skill)
        {
            Skill += skill;
        }

        public void AddAffinity(EElement element, float value)
        {
            switch (element)
            {
                case EElement.Fire :
                    Affinity += new Affinity(value, 0, 0);
                    break;
                case EElement.Nature:
                    Affinity += new Affinity(0,value, 0);
                    break;
                case EElement.Water:
                    Affinity += new Affinity(0,0,value);
                    break;
                default: break;
            }
        }

        public void SetAffinity(Affinity _affinity)
        {
            Affinity = new Affinity(_affinity);
        }

        public static Power Randomize(Power min, Power max)
        {
            Power ret = new Power
            {
                Basic = Random.Range(min.Basic, max.Basic),
                Skill = Random.Range(min.Skill, max.Skill),
                Spell = Random.Range(min.Spell, max.Spell),
                Affinity = new Affinity(Random.Range(min.Affinity.Fire, max.Affinity.Fire),
                    Random.Range(min.Affinity.Nature, max.Affinity.Nature),
                    Random.Range(min.Affinity.Water, max.Affinity.Water))
            };

            return ret;
        }
    }
}