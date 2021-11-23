using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _Instances;
using Cells;
using Gears;
using Grid;
using Skills;
using Skills._Zone;
using Stats;
using StatusEffect;
using UnityEngine;

namespace Units
{
    /// <summary>
    /// Sub-Class of Units that Link a Monster (Scriptable Object) to a Prefab
    /// </summary>
    [RequireComponent(typeof(Deck))]
    public class Monster : Unit
    {
        [Header("Monster Special")]
        [SerializeField] private Deck deck;
        [SerializeField] private SkillInfo skill;
        
        [Header("Event Sender")]
        [SerializeField] private UnitEvent onDeathLoot;
        [SerializeField] private UnitEvent onDeathRelic;
        [SerializeField] private VoidEvent onActionDone;
        public bool isPlaying;

        public SkillSO Skill => skill.Skill;

        public List<RelicSO> Relics { get; private set; }
        public EReward RewardType { get; private set; }
        public EMonster Type { get; private set; }
        
        /// <summary>
        /// Method called at the Unit Instantiation, it's create the Stats and change the Sprite. 
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="Stage"></param>
        public void Spawn(MonsterSO monster, int Stage)
        {
            UnitName = monster.Name;
            unitSprite.sprite = monster.UnitSprite;
            RewardType = monster.RewardType;
            Type = monster.Type;
            Relics = new List<RelicSO>();
            if (monster.Relic != null)
            {
                Relics.Add(monster.Relic);
            }

            buffs = new List<Buff>();
            
            Inventory = new Inventory();
            if (RewardType == EReward.Gear)
            {
                Inventory.GenerateGearFor(monster, Stage);
            }

            if (monster.Type == EMonster.Boss)
            {
                Relics.Add(DataBase.Relic.GetRandom());
            }

            baseStats = monster.Stats();
            BattleStats = new BattleStats(baseStats + Inventory.GearStats());
            total = BattleStats;
            
            if (monster.Skill != null)
                deck.InitializeForMonster(monster.Skill, Relics);
            else
            {
                deck.InitializeForMonster(DataBase.Skill.GetSkillFor(monster), Relics);
            }
            
            skill.UpdateSkill(0,this);

            InitializeSprite();
        }

        public override bool IsUnitAttackable(Unit other, Cell sourceCell)
        {
            return Zone.GetRange(BattleStats.Range + DataBase.Skill.MonsterAttack.Range, cell).Contains(other.Cell);
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            skill.UpdateSkill(0, this);
        }

        public void ShowRange()
        {
            foreach (Cell _cell in BattleStateManager.instance.Cells)
            {           
                _cell.UnMark();
            }
            foreach (Cell _cellInRange in Zone.GetRange(skill.Range, Cell))
            {
                _cellInRange.MarkAsUnReachable();
            }

            foreach (Cell _cell in Zone.CellsInView(skill, cell))
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
                yield return new WaitForSeconds(0.1f);
                GameObject inventory = GameObject.Find("InventoryUI");
                while (inventory.activeSelf)
                    yield return null;
                onActionDone.Raise();
            }
            if (RewardType == EReward.Relic || Type == EMonster.Boss)
            {
                onDeathRelic.Raise(this);
                yield return new WaitForSeconds(0.3f);
                GameObject relicChoice = GameObject.Find("RelicChoiceUI");
                while (relicChoice.activeSelf)
                    yield return null;
                onActionDone.Raise();
            }
            //TODO : Skill on death

            yield return base.OnDestroyed();
        }
    }
}