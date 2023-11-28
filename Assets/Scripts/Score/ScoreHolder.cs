using System;
using System.Collections.Generic;
using _Extension;
using Players;
using Units;

namespace Score
{
    public static class ScoreHolder
    {
        public static List<MonsterSo> Bosses { get; private set; } = new List<MonsterSo>();
        public static int DamageDealtTotal { get; private set; } = 0;
        public static int DamageDealtBiggest { get; private set; } = 0;
        public static int DamageTakenTotal { get; private set; } = 0;
        public static int DamageTakenBiggest { get; private set; } = 0;
        public static int CellWalked { get; private set; } = 0;
        public static int GearSalvaged { get; private set; } = 0;
        public static int CraftingMaterialCollected { get; private set; } = 0;

        public static void AddBoss(MonsterSo _monster)
        {
            if (_monster.Type != EMonster.Boss) return;
            Bosses.Add(_monster);
        }

        public static void Damage(object _sender, EventArgs _e)
        {
            AttackEventArgs _attackEvent = (AttackEventArgs) _e;
            if (_attackEvent.Defender == _attackEvent.Attacker) return;
            if (_attackEvent.Attacker.playerType == EPlayerType.Human)
            {
                DamageDealtTotal += _attackEvent.Damage;
                DamageDealtBiggest = DamageDealtBiggest.Max(_attackEvent.Damage);
            }
            if (_attackEvent.Defender.playerType == EPlayerType.Human)
            {
                DamageTakenTotal += _attackEvent.Damage;
                DamageTakenBiggest = DamageTakenBiggest.Max(_attackEvent.Damage);
            }
        }

        public static void AddDistance(int _pathCount)
        {
            CellWalked += _pathCount;
        }

        public static void AddGearSalvaged()
        {
            GearSalvaged++;
        }

        public static void AddCraftingMaterial(int _number)
        {
            CraftingMaterialCollected += _number;
        }

        public static int GameScore()
        {
            int _score = CellWalked * 3
                        + Bosses.Count * 100
                        + DamageDealtTotal
                        + DamageDealtBiggest * 10
                        - DamageTakenTotal / 2
                        - DamageTakenBiggest * 5
                        + GearSalvaged * 7
                        + CraftingMaterialCollected * 2;
            return _score;
        }
    }
}