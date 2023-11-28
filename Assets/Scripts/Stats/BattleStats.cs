using System;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Stats
{
    /// <summary>
    /// All Stats that are Used in Battle Mod
    /// </summary>
    [Serializable]
    public struct BattleStats
    {
        public int HP;
        public int Shield;
        public int Speed;
        public int Power;
        public int Focus;
        public Affinity Affinity;
        public float MP;
        public float AP;
        [FormerlySerializedAs("Range")] public GridRange gridRange;

        public float GetDamageTaken(float _damage, EElement _element) =>
            Math.Max(1, _damage - (_damage * Affinity.GetAffinity(_element) / 50));
        
        public int GetHealTaken(float _heal, EElement _element) => (int)(_heal + (_heal * Affinity.GetAffinity(_element) / 50));

        public BattleStats(BattleStats _battleStats)
        {
            HP = _battleStats.HP;
            Shield = _battleStats.Shield;
            Speed = _battleStats.Speed;
            Power = _battleStats.Power;
            Focus = _battleStats.Focus;
            Affinity = _battleStats.Affinity;
            MP = _battleStats.MP;
            AP = _battleStats.AP;
            gridRange = _battleStats.gridRange;
        }

        public static BattleStats operator +(BattleStats a, BattleStats b)
        {
            BattleStats res = new BattleStats(a);
            res.HP += b.HP;
            res.Shield += b.Shield;
            res.Speed += b.Speed;
            res.Power += b.Power;
            res.Focus += b.Focus;
            res.Affinity += b.Affinity;
            res.MP += b.MP;
            res.AP += b.AP;
            res.gridRange += b.gridRange;
            return res;
        }
        
        public static BattleStats operator -(BattleStats a, BattleStats b)
        {
            BattleStats res = new BattleStats(a);
            res.HP -= b.HP;
            res.Shield -= b.Shield;
            res.Speed -= b.Speed;
            res.Power -= b.Power;
            res.Focus -= b.Focus;
            res.Affinity -= b.Affinity;
            res.MP -= b.MP;
            res.AP -= b.AP;
            res.gridRange -= b.gridRange;
            return res;
        }
        
        public static BattleStats operator *(BattleStats a, BattleStats b)
        {
            BattleStats res = new BattleStats(a);
            res.HP *= b.HP;
            res.Shield *= b.Shield;
            res.Speed *= b.Speed;
            res.Power *= b.Power;
            res.Focus *= b.Focus;
            res.Affinity *= b.Affinity;
            res.MP *= b.MP;
            res.AP *= b.AP;
            res.gridRange *= b.gridRange;
            return res;
        }
        
        public static BattleStats operator +(BattleStats a, float b)
        {
            BattleStats res = new BattleStats(a);
            res.HP += (int)b;
            res.Shield += (int)b;
            res.Speed += (int)b;
            res.Power += (int)b;
            res.Focus += (int)b;
            res.Affinity += (int) b;
            res.MP += b;
            res.AP += b;
            res.gridRange += b;
            return res;
        }
        
        public static BattleStats operator *(BattleStats a, float b)
        {
            BattleStats res = new BattleStats(a);
            res.HP = (int)(res.HP * b);
            res.Shield = (int)(res.Shield * b);
            res.Speed = (int)(res.Speed * b);
            res.Power = (int)(res.Power * b);
            res.Focus = (int)(res.Focus * b);
            res.Affinity *= b;
            res.MP *= b;
            res.AP *= b;
            res.gridRange *= b;
            return res;
        }

        public static BattleStats Randomize(BattleStats min, BattleStats max)
        {
            BattleStats ret = new BattleStats()
            {
                HP = Random.Range(min.HP, max.HP),
                Shield = Random.Range(min.Shield, max.Shield),
                Speed = Random.Range(min.Speed, max.Speed),
                Power = Random.Range(min.Power, max.Power),
                Focus = Random.Range(min.Focus, max.Focus),
                Affinity = Affinity.Random(max.Affinity, max.Affinity),
                MP = Random.Range(min.MP, max.MP),
                AP = Random.Range(min.AP, max.AP),
                gridRange = GridRange.Randomize(min.gridRange, max.gridRange)
            };

            return ret;
        }
        
        public BattleStats(float a)
        {
            HP = (int) a;
            Shield = (int) a;
            Speed = (int) a;
            Power = (int) a;
            Focus = (int) a;
            Affinity = new Affinity(a);
            MP = a;
            AP = a;
            gridRange = new GridRange(a);
        }

        public void Randomize(float min, float max)
        {
            Randomize(new BattleStats(min), new BattleStats(max));
        }

        public void Randomize(float a)
        {
            Randomize(0,a);
        }

        public int GetPower(EElement element)
        {
            return (int) (Power + Affinity.GetAffinity(element));
        }
        
        public int GetFocus()
        {
            return Focus;
        }
    }
}