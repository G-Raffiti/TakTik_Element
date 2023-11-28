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
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("onSkillTooltip_ON")]
        [SerializeField] private SkillEvent onSkillTooltipOn;
        
        [Header("Event Listener")]
        [SerializeField] private UnitEvent onInventoryClosed;
        
        [FormerlySerializedAs("onMonsterTooltip_ON")]
        [Header("Tooltip Event for MONSTER")]
        [SerializeField] private UnitEvent onMonsterTooltipOn;
        public override UnitEvent OnTooltipOn => onMonsterTooltipOn;
        
        public bool isPlaying;

        public MonsterSo MonsterSo { get; private set; }
        public Skill MonsterSkill { get; private set; }
        public Archetype Archetype { get; private set; }
        public List<RelicSo> Relics { get; private set; }
        public EReward RewardType { get; private set; }
        public EMonster Type { get; private set; }
        public override Sprite UnitSprite => MonsterSo.UnitSprite;

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
            onSkillTooltipOn.Raise(skill);
        }


        /// <summary>
        /// Method called at the Unit Instantiation, it's create the Stats and change the Sprite. 
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="Stage"></param>
        public void Spawn(MonsterSo _monster, int _stage)
        {
            MonsterSo = _monster;
            unitName = _monster.Name;
            unitSpriteRenderer.sprite = _monster.UnitSprite;
            RewardType = _monster.RewardType;
            Type = _monster.Type;
            Archetype = _monster.Archetype;
            
            Relics = new List<RelicSo>();
            if (_monster.Relic != null)
            {
                Relics.Add(_monster.Relic);
            }

            buffs = new List<Buff>();
            
            inventory = new Inventory();
            if(BattleStage.Stage >= 0) 
                inventory.GenerateGearFor(_monster);

            if (_monster.Type == EMonster.Boss && BattleStage.Stage >= 0)
            {
                Relics.Add(DataBase.Relic.GetRandom());
            }

            deck.relics = new List<RelicSo>(Relics);
            deck.UpdateDeck();

            baseStats = _monster.Stats();
            battleStats = new BattleStats(baseStats + inventory.GearStats());
            total = battleStats;
            if (_monster.Skill == null)
                MonsterSkill = Skill.CreateSkill(DataBase.Skill.GetSkillFor(_monster), deck, this);
            else MonsterSkill = Skill.CreateSkill(_monster.Skill, deck, this);

            if (MonsterSkill.Cost > total.ap)
                total.ap = MonsterSkill.Cost;
            
            skill.skill = MonsterSkill;
            skill.unit = this;

            MainElement();
            InitializeSprite();
        }

        public override Relic Relic => new Relic();

        public override void EndTurn()
        {
            base.EndTurn();
            skill.skill = MonsterSkill;
        }

        public override string GetInfoMain()
        {
            return $"{ColouredName()} {MonsterSo.Archetype.Type}";
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
            IsDying = true;
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
        
        private void DestroyUnit(Unit _unit)
        {
            if (BattleStateManager.instance.DeadThisTurn.Count == 0)
                BattleStateManager.instance.Check();
            if (_unit == this)
                Destroy(this);
        }
    }
}