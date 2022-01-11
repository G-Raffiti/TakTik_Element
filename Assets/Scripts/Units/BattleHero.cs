﻿using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using Gears;
using Relics;
using StateMachine;
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
        [SerializeField] private IntEvent onCellWalked;
        
        private Hero hero;

        public Hero Hero => hero;
        public override Relic Relic { get => hero.GetRelic(); } 

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
        
        public override string GetInfoDown()
        {
            string str = Buffs.Aggregate("", (_current, _buff) => _current + (_buff.InfoOnUnit(_buff, this) + "\n"));
            foreach (RelicSO _heroRelic in Hero.Relics)
            {
                str += _heroRelic.Name + "\n";
            }

            return str; 
        }

        protected override void OnMoveFinished(int _cellWalked)
        {
            base.OnMoveFinished(_cellWalked);
            onCellWalked.Raise(_cellWalked);
        }
    }
}