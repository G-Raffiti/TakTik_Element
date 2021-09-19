using System.Collections.Generic;
using Gears;
using Stats;
using StatusEffect;
using UnityEngine;

namespace Units
{
    /// <summary>
    /// Sub-Class of Units that Link a Hero (MonoBehaviour) to a Prefab
    /// </summary>
    public class BattleHero : Unit
    {
        private Hero hero;
        
        public void Spawn(Hero _hero)
        {
            hero = _hero;
            
            UnitName = _hero.UnitName;

            Inventory = new Inventory() {gears = new List<Gear>()};
            Inventory.gears.AddRange(_hero.Inventory.gears);
            
            baseStats = _hero.BattleStats;
            BattleStats = new BattleStats(baseStats + Inventory.GearStats());
            total = BattleStats;
            BattleStats.HP = hero.ActualHP;

            unitSprite.sprite = _hero.UnitSprite;

            buffs = new List<Buff>();
            
            InitializeSprite();
        }

        public override void UpdateStats()
        {
            base.UpdateStats();
            hero.Inventory.gears = new List<Gear>(Inventory.gears);
        }
    }
}