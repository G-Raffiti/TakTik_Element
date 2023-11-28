using System;
using System.Collections;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _Instances;
using _ScriptableObject;
using Buffs;
using Cells;
using Decks;
using Gears;
using Relics;
using Skills;
using Skills._Zone;
using StateMachine;
using Stats;
using UnityEngine;

namespace Units
{
    /// <summary>
    /// Sub-Class of Units that Link a Monster (Scriptable Object) to a Prefab
    /// </summary>
    [RequireComponent(typeof(DeckMono))]
    public class Monster : Unit
    {
        [Header("Monster Special")]
        [SerializeField] private DeckMono deck;
        [SerializeField] private SkillInfo skill;
        
        [Header("Event Sender")]
        [SerializeField] private UnitEvent onDeathLoot;
        [SerializeField] private UnitEvent onDeathRelic;
        [SerializeField] private SkillEvent onSkillTooltip_ON;
        
        [Header("Event Listener")]
        [SerializeField] private UnitEvent onInventoryClosed;
        
        [Header("Tooltip Event for MONSTER")]
        [SerializeField] private UnitEvent onMonsterTooltip_ON;
        public override UnitEvent onTooltip_ON => onMonsterTooltip_ON;
        
        public bool isPlaying;

        public MonsterSO MonsterSO { get; private set; }
        public Skill monsterSkill { get; private set; }
        public Archetype Archetype { get; private set; }
        public List<RelicSO> Relics { get; private set; }
        public EReward RewardType { get; private set; }
        public EMonster Type { get; private set; }
        public override Sprite UnitSprite => MonsterSO.UnitSprite;

        private void Start()
        {
            onInventoryClosed.EventListeners += DestroyUnit;
        }

        private void OnDisable()
        {
            onInventoryClosed.EventListeners += DestroyUnit;
        }

        public override void OnRightClick()
        {
            base.OnRightClick();
            onSkillTooltip_ON.Raise(skill);
        }


        /// <summary>
        /// Method called at the Unit Instantiation, it's create the Stats and change the Sprite. 
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="Stage"></param>
        public void Spawn(MonsterSO monster, int Stage)
        {
            MonsterSO = monster;
            UnitName = monster.Name;
            unitSpriteRenderer.sprite = monster.UnitSprite;
            RewardType = monster.RewardType;
            Type = monster.Type;
            Archetype = monster.Archetype;
            
            Relics = new List<RelicSO>();
            if (monster.Relic != null)
            {
                Relics.Add(monster.Relic);
            }

            buffs = new List<Buff>();
            
            Inventory = new Inventory();
            if(BattleStage.Stage >= 0) 
                Inventory.GenerateGearFor(monster);

            if (monster.Type == EMonster.Boss && BattleStage.Stage >= 0)
            {
                Relics.Add(DataBase.Relic.GetRandom());
            }

            deck.Relics = new List<RelicSO>(Relics);
            deck.UpdateDeck();

            baseStats = monster.Stats();
            BattleStats = new BattleStats(baseStats + Inventory.GearStats());
            total = BattleStats;
            if (monster.Skill == null)
                monsterSkill = Skill.CreateSkill(DataBase.Skill.GetSkillFor(monster), deck, this);
            else monsterSkill = Skill.CreateSkill(monster.Skill, deck, this);

            if (monsterSkill.Cost > total.AP)
                total.AP = monsterSkill.Cost;
            
            skill.skill = monsterSkill;
            skill.Unit = this;

            MainElement();
            InitializeSprite();
        }

        public override Relic Relic => new Relic();

        public override void EndTurn()
        {
            base.EndTurn();
            skill.skill = monsterSkill;
        }

        public override string GetInfoMain()
        {
            return $"{ColouredName()} {MonsterSO.Archetype.Type}";
        }

        public void ShowRange()
        {
            foreach (Cell _cell in BattleStateManager.instance.Cells)
            {           
                _cell.UnMark();
            }
            foreach (Cell _cellInRange in Zone.GetRange(skill.skill.GridRange, Cell))
            {
                _cellInRange.MarkAsUnReachable();
            }

            foreach (Cell _cell in Zone.CellsInView(skill.skill, Cell))
            {
                _cell.MarkAsInteractable();
            }
        }
        
        public override IEnumerator OnDestroyed()
        {
            isDying = true;
            if (RewardType == EReward.Gear && BattleStateManager.instance.PlayingUnit is BattleHero)
            {
                onDeathLoot.Raise(this);
            }
            if (RewardType == EReward.Relic || Type == EMonster.Boss)
            {
                onDeathRelic.Raise(this);
            }
            //TODO : Skill on death

            yield return base.OnDestroyed();
        }
        
        private void DestroyUnit(Unit unit)
        {
            if (BattleStateManager.instance.DeadThisTurn.Count == 0)
                BattleStateManager.instance.Check();
            if (unit == this)
                Destroy(this);
        }
    }
}