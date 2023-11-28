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
        [FormerlySerializedAs("HP")]
        public int hp;
        [FormerlySerializedAs("Shield")]
        public int shield;
        [FormerlySerializedAs("Speed")]
        public int speed;
        [FormerlySerializedAs("Power")]
        public int power;
        [FormerlySerializedAs("Focus")]
        public int focus;
        [FormerlySerializedAs("Affinity")]
        public Affinity affinity;
        [FormerlySerializedAs("MP")]
        public float mp;
        [FormerlySerializedAs("AP")]
        public float ap;
        [FormerlySerializedAs("Range")] public GridRange gridRange;

        public float GetDamageTaken(float _damage, EElement _element) =>
            Math.Max(1, _damage - (_damage * affinity.GetAffinity(_element) / 50));
        
        public int GetHealTaken(float _heal, EElement _element) => (int)(_heal + (_heal * affinity.GetAffinity(_element) / 50));

        public BattleStats(BattleStats _battleStats)
        {
            hp = _battleStats.hp;
            shield = _battleStats.shield;
            speed = _battleStats.speed;
            power = _battleStats.power;
            focus = _battleStats.focus;
            affinity = _battleStats.affinity;
            mp = _battleStats.mp;
            ap = _battleStats.ap;
            gridRange = _battleStats.gridRange;
        }

        public static BattleStats operator +(BattleStats _a, BattleStats _b)
        {
            BattleStats _res = new BattleStats(_a);
            _res.hp += _b.hp;
            _res.shield += _b.shield;
            _res.speed += _b.speed;
            _res.power += _b.power;
            _res.focus += _b.focus;
            _res.affinity += _b.affinity;
            _res.mp += _b.mp;
            _res.ap += _b.ap;
            _res.gridRange += _b.gridRange;
            return _res;
        }
        
        public static BattleStats operator -(BattleStats _a, BattleStats _b)
        {
            BattleStats _res = new BattleStats(_a);
            _res.hp -= _b.hp;
            _res.shield -= _b.shield;
            _res.speed -= _b.speed;
            _res.power -= _b.power;
            _res.focus -= _b.focus;
            _res.affinity -= _b.affinity;
            _res.mp -= _b.mp;
            _res.ap -= _b.ap;
            _res.gridRange -= _b.gridRange;
            return _res;
        }
        
        public static BattleStats operator *(BattleStats _a, BattleStats _b)
        {
            BattleStats _res = new BattleStats(_a);
            _res.hp *= _b.hp;
            _res.shield *= _b.shield;
            _res.speed *= _b.speed;
            _res.power *= _b.power;
            _res.focus *= _b.focus;
            _res.affinity *= _b.affinity;
            _res.mp *= _b.mp;
            _res.ap *= _b.ap;
            _res.gridRange *= _b.gridRange;
            return _res;
        }
        
        public static BattleStats operator +(BattleStats _a, float _b)
        {
            BattleStats _res = new BattleStats(_a);
            _res.hp += (int)_b;
            _res.shield += (int)_b;
            _res.speed += (int)_b;
            _res.power += (int)_b;
            _res.focus += (int)_b;
            _res.affinity += (int) _b;
            _res.mp += _b;
            _res.ap += _b;
            _res.gridRange += _b;
            return _res;
        }
        
        public static BattleStats operator *(BattleStats _a, float _b)
        {
            BattleStats _res = new BattleStats(_a);
            _res.hp = (int)(_res.hp * _b);
            _res.shield = (int)(_res.shield * _b);
            _res.speed = (int)(_res.speed * _b);
            _res.power = (int)(_res.power * _b);
            _res.focus = (int)(_res.focus * _b);
            _res.affinity *= _b;
            _res.mp *= _b;
            _res.ap *= _b;
            _res.gridRange *= _b;
            return _res;
        }

        public static BattleStats Randomize(BattleStats _min, BattleStats _max)
        {
            BattleStats _ret = new BattleStats()
            {
                hp = Random.Range(_min.hp, _max.hp),
                shield = Random.Range(_min.shield, _max.shield),
                speed = Random.Range(_min.speed, _max.speed),
                power = Random.Range(_min.power, _max.power),
                focus = Random.Range(_min.focus, _max.focus),
                affinity = Affinity.Random(_max.affinity, _max.affinity),
                mp = Random.Range(_min.mp, _max.mp),
                ap = Random.Range(_min.ap, _max.ap),
                gridRange = GridRange.Randomize(_min.gridRange, _max.gridRange)
            };

            return _ret;
        }
        
        public BattleStats(float _a)
        {
            hp = (int) _a;
            shield = (int) _a;
            speed = (int) _a;
            power = (int) _a;
            focus = (int) _a;
            affinity = new Affinity(_a);
            mp = _a;
            ap = _a;
            gridRange = new GridRange(_a);
        }

        public void Randomize(float _min, float _max)
        {
            Randomize(new BattleStats(_min), new BattleStats(_max));
        }

        public void Randomize(float _a)
        {
            Randomize(0,_a);
        }

        public int GetPower(EElement _element)
        {
            return (int) (power + affinity.GetAffinity(_element));
        }
        
        public int GetFocus()
        {
            return focus;
        }
    }
}