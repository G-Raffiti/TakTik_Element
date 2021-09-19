using System;
using _ScriptableObject;
using Skills._Zone;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Stats
{
    /// <summary>
    /// All Stats that are Used in Battle Mod
    /// </summary>
    [Serializable]
    public struct BattleStats
    {
        [HideInInspector] public int TurnPoint;

        public int HP;
        public int Shield;
        public float Dodge;
        public int Speed;
        public Power Power;
        public float MP;
        public float AP;
        public Range Range;
        public int Focus;

        public int GetFocus(EElement _element) => (int)(Focus * Power.Affinity.GetAffinity(_element));

        public float GetDamageTaken(EElement _element) =>
            (int) (((1f / Power.Affinity.GetAffinity(_element) + 1f) / 2f) * 100f);

        public BattleStats(BattleStats _battleStats)
        {
            TurnPoint = _battleStats.TurnPoint;
            HP = _battleStats.HP;
            Shield = _battleStats.Shield;
            Dodge = _battleStats.Dodge;
            Speed = _battleStats.Speed;
            Power = _battleStats.Power;
            MP = _battleStats.MP;
            AP = _battleStats.AP;
            Range = _battleStats.Range;
            Focus = _battleStats.Focus;
        }

        public static BattleStats operator +(BattleStats a, BattleStats b)
        {
            BattleStats res = new BattleStats(a);
            res.TurnPoint += b.TurnPoint;
            res.HP += b.HP;
            res.Shield += b.Shield;
            res.Dodge += b.Dodge;
            res.Speed += b.Speed;
            res.Power += b.Power;
            res.MP += b.MP;
            res.AP += b.AP;
            res.Range += b.Range;
            res.Focus += b.Focus;
            return res;
        }
        
        public static BattleStats operator -(BattleStats a, BattleStats b)
        {
            BattleStats res = new BattleStats(a);
            res.TurnPoint -= b.TurnPoint;
            res.HP -= b.HP;
            res.Shield -= b.Shield;
            res.Dodge -= b.Dodge;
            res.Speed -= b.Speed;
            res.Power -= b.Power;
            res.MP -= b.MP;
            res.AP -= b.AP;
            res.Range -= b.Range;
            res.Focus -= b.Focus;
            return res;
        }
        
        public static BattleStats operator *(BattleStats a, BattleStats b)
        {
            BattleStats res = new BattleStats(a);
            res.TurnPoint *= b.TurnPoint;
            res.HP *= b.HP;
            res.Shield *= b.Shield;
            res.Dodge *= b.Dodge;
            res.Speed *= b.Speed;
            res.Power *= b.Power;
            res.MP *= b.MP;
            res.AP *= b.AP;
            res.Range *= b.Range;
            res.Focus *= b.Focus;
            return res;
        }
        
        public static BattleStats operator +(BattleStats a, float b)
        {
            BattleStats res = new BattleStats(a);
            res.TurnPoint += (int)b;
            res.HP += (int)b;
            res.Shield += (int)b;
            res.Dodge += b;
            res.Speed += (int)b;
            res.Power += (int)b;
            res.MP += b;
            res.AP += b;
            res.Range += b;
            res.Focus += (int)b;
            return res;
        }
        
        public static BattleStats operator *(BattleStats a, float b)
        {
            BattleStats res = new BattleStats(a);
            res.TurnPoint = (int)(res.TurnPoint * b);
            res.HP = (int)(res.HP * b);
            res.Shield = (int)(res.Shield * b);
            res.Dodge *= b;
            res.Speed = (int)(res.Speed * b);
            res.Power *= b;
            res.MP *= b;
            res.AP *= b;
            res.Range *= b;
            res.Focus = (int)(res.Focus * b);
            return res;
        }

        public static BattleStats Randomize(BattleStats min, BattleStats max)
        {
            BattleStats ret = new BattleStats()
            {
                TurnPoint = Random.Range(min.TurnPoint, max.TurnPoint),
                HP = Random.Range(min.HP, max.HP),
                Shield = Random.Range(min.Shield, max.Shield),
                Dodge = Random.Range(min.Dodge, max.Dodge),
                Speed = Random.Range(min.Speed, max.Speed),
                Power = Power.Randomize(min.Power, max.Power),
                MP = Random.Range(min.MP, max.MP),
                AP = Random.Range(min.AP, max.AP),
                Range = Range.Randomize(min.Range, max.Range)
            };

            return ret;
        }
        
        public BattleStats(float a)
        {
            TurnPoint = (int) a;
            HP = (int) a;
            Shield = (int) a;
            Dodge = a;
            Speed = (int) a;
            Power = new Power((int) a);
            MP = a;
            AP = a;
            Range = new Range(a);
            Focus = (int) a;
        }

        public void Randomize(float min, float max)
        {
            Randomize(new BattleStats(min), new BattleStats(max));
        }

        public void Randomize(float a)
        {
            Randomize(0,a);
        }
    }
}