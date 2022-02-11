using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using Buffs;
using Gears;
using Relics;
using StateMachine;
using StateMachine.GridStates;
using Stats;
using UnityEngine;

namespace Units
{
    /// <summary>
    /// Sub-Class of Units that Link a Hero (MonoBehaviour) to a Prefab
    /// </summary>
    public class BattleHero : Unit
    {
        [Header("Battle Hero Events Sender")]
        [SerializeField] private UnitEvent onHeroSelected;
        [SerializeField] private IntEvent onCellWalked;

        [Header("Tooltip Event for HERO")]
        [SerializeField] private UnitEvent onHeroTooltip_ON;
        public override UnitEvent onTooltip_ON => onHeroTooltip_ON;
        
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
            BattleStats = new BattleStats(hero.TotalStats);
            total = new BattleStats(BattleStats);
            BattleStats.HP = hero.ActualHP;
            buffs = new List<Buff>();

            if (unitSpriteRenderer == null) return;
            
            unitSpriteRenderer.sprite = _hero.UnitSprite;
            InitializeSprite();
        }

        public override void UpdateStats()
        {
            base.UpdateStats();
            hero.Inventory.gears = new List<Gear>(Inventory.gears);
            hero.UpdateHP();
        }

        public override string GetInfoMain()
        {
            return $"{ColouredName()}";
        }

        public override void OnLeftClick()
        {
            base.OnLeftClick();
            if (BattleStateManager.instance.BattleState.State == EBattleState.Beginning)
                onHeroSelected.Raise(this);
        }

        protected override void OnMoveFinished(int _cellWalked)
        {
            base.OnMoveFinished(_cellWalked);
            onCellWalked.Raise(_cellWalked);
        }
    }
}