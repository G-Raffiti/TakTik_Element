using System.Collections.Generic;
using _EventSystem.CustomEvents;
using Gears;
using Grid;
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
        [SerializeField] private UnitEvent onHeroSelected;
        private Hero hero;

        public Hero Hero => hero;

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
            hero.UpdateHP();
        }

        public override void OnMouseDown()
        {
            base.OnMouseDown();
            if (!BattleStateManager.instance.GameStarted)
                onHeroSelected.Raise(this);
        }
    }
}