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
using UnityEngine.Serialization;

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

        [FormerlySerializedAs("onHeroTooltip_ON")]
        [Header("Tooltip Event for HERO")]
        [SerializeField] private UnitEvent onHeroTooltipOn;
        public override UnitEvent OnTooltipOn => onHeroTooltipOn;
        
        private Hero hero;

        public Hero Hero => hero;
        public override Relic Relic { get => hero.GetRelic(); }
        public override Sprite UnitSprite => hero.UnitSprite;

        public void Spawn(Hero _hero)
        {
            hero = _hero;
            
            unitName = _hero.UnitName;

            inventory = new Inventory() {gears = new List<Gear>()};
            inventory.gears.AddRange(_hero.Inventory.gears);
            
            baseStats = _hero.BattleStats;
            battleStats = new BattleStats(hero.TotalStats);
            total = new BattleStats(battleStats);
            battleStats.hp = hero.ActualHp;
            buffs = new List<Buff>();

            if (unitSpriteRenderer == null) return;
            
            unitSpriteRenderer.sprite = _hero.UnitSprite;
            
            MainElement();
            InitializeSprite();
        }

        public override void UpdateStats()
        {
            base.UpdateStats();
            hero.Inventory.gears = new List<Gear>(inventory.gears);
            hero.UpdateHp();
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