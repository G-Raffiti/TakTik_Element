using System;
using System.Collections;
using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _Instances;
using _ScriptableObject;
using Cells;
using Decks;
using Gears;
using Relics;
using Skills;
using Skills._Zone;
using StateMachine;
using Stats;
using StatusEffect;
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
        
        [Header("Event Listener")]
        [SerializeField] private UnitEvent onInventoryClosed;
        
        public bool isPlaying;

        public MonsterSO MonsterSO { get; private set; }


        public Skill monsterSkill { get; private set; }

        public Archetype Archetype { get; private set; }
        public List<RelicSO> Relics { get; private set; }
        public EReward RewardType { get; private set; }
        public EMonster Type { get; private set; }

        private void Start()
        {
            onInventoryClosed.EventListeners += DestroyUnit;
        }

        private void OnDisable()
        {
            onInventoryClosed.EventListeners += DestroyUnit;
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
            unitSprite.sprite = monster.UnitSprite;
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
            if(KeepBetweenScene.Stage >= 0) 
                Inventory.GenerateGearFor(monster);

            if (monster.Type == EMonster.Boss && KeepBetweenScene.Stage >= 0)
            {
                Relics.Add(DataBase.Relic.GetRandom());
            }

            deck.Relics = new List<RelicSO>(Relics);
            deck.UpdateDeck();

            baseStats = monster.Stats();
            BattleStats = new BattleStats(baseStats + Inventory.GearStats());
            total = BattleStats;
            monsterSkill = Skill.CreateSkill(DataBase.Skill.GetSkillFor(monster), deck, this);

            if (monsterSkill.Cost > total.AP)
                total.AP = monsterSkill.Cost;
            
            skill.skill = monsterSkill;
            skill.Unit = this;

            InitializeSprite();
        }

        public override Relic Relic => new Relic();

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            skill.skill = monsterSkill;
        }

        public void ShowRange()
        {
            foreach (Cell _cell in BattleStateManager.instance.Cells)
            {           
                _cell.UnMark();
            }
            foreach (Cell _cellInRange in Zone.GetRange(skill.skill.Range, Cell))
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