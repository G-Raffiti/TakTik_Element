using System;
using System.Collections.Generic;
using _Extension;
using Units;

namespace Score
{
    public static class ScoreHolder
    {
        public static List<MonsterSO> Bosses { get; private set; } = new List<MonsterSO>();
        public static int DamageDealtTotal { get; private set; } = 0;
        public static int DamageDealtBiggest { get; private set; } = 0;
        public static int DamageTakenTotal { get; private set; } = 0;
        public static int DamageTakenBiggest { get; private set; } = 0;
        public static int CellWalked { get; private set; } = 0;
        public static int GearSalvaged { get; private set; } = 0;
        public static int CraftingMaterialCollected { get; private set; } = 0;

        public static void AddBoss(MonsterSO _monster)
        {
            if (_monster.Type != EMonster.Boss) return;
            Bosses.Add(_monster);
        }

        public static void Damage(object sender, EventArgs _e)
        {
            AttackEventArgs e = (AttackEventArgs) _e;
            if (e.Defender == e.Attacker) return;
            if (e.Attacker.playerNumber == 0)
            {
                DamageDealtTotal += e.Damage;
                DamageDealtBiggest = DamageDealtBiggest.Max(e.Damage);
            }
            if (e.Defender.playerNumber == 0)
            {
                DamageTakenTotal += e.Damage;
                DamageTakenBiggest = DamageTakenBiggest.Max(e.Damage);
            }
        }

        public static void AddDistance(int pathCount)
        {
            CellWalked += pathCount;
        }

        public static void AddGearSalvaged()
        {
            GearSalvaged++;
        }

        public static void AddCraftingMaterial(int number)
        {
            CraftingMaterialCollected += number;
        }
    }
}