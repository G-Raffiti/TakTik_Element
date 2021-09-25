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
        [SerializeField] private UnitEvent onDeathLoot;
        [SerializeField] private UnitEvent onDeathRelic;
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
            deck.Initialize(monster.Skill, Relics);
            
            skill.CreateSkill(this);
            
            
            
            InitializeSprite();
        }
        
        /// <summary>
        /// Check if the "other" is in Range of the Monster's Skill
        /// </summary>
        /// <param name="other">
        /// other is the Target to damage
        /// </param>
        /// <param name="sourceCell">
        /// the Cell from where the test is done
        /// </param>
        /// <returns></returns>
        public bool IsUnitTargetable(Unit other, Cell sourceCell)
        {
            List<Cell> inRange = new List<Cell>();
            foreach (Cell _cell in Zone.GetRange(skill.Range, sourceCell))
            {
                inRange.AddRange(skill.GetZoneOfEffect(_cell).Except(inRange));
            }

            return inRange.Contains(other.Cell);
        }

        /// <summary>
        /// Try to use the Monsters Skill without damaging any ally
        /// </summary>
        /// <param name="_target"></param>
        /// <param name="_myUnits"></param>
        public void UseSkill(Unit _target, List<Unit> _myUnits)
        {
            if (!IsUnitTargetable(_target, Cell)) return;
            Cell _cell = null;
            bool ally = false;
            foreach (Cell _cellInRange in Zone.GetRange(skill.Range, Cell))
            {
                _myUnits.ForEach(u =>
                {
                    if (skill.GetZoneOfEffect(_cellInRange).Contains(u.Cell))
                        ally = true;
                });
                if (skill.GetZoneOfEffect(_cellInRange).Contains(_target.Cell) && (_cell == null || !ally))
                    _cell = _cellInRange;
            }
            
            skill.UseSkill(_cell);
        }

        public override IEnumerator OnDestroyed()
        {
            isDying = true;
            if (RewardType == EReward.Gear && BattleStateManager.instance.PlayingUnit is BattleHero)
            {
                onDeathLoot.Raise(this);
                yield return new WaitForSeconds(0.1f);
                GameObject inventory = GameObject.Find("InventoryUI");
                BattleStateManager.instance.BlockInputs();
                while (inventory.activeSelf)
                    yield return null;
                BattleStateManager.instance.OnSkillUsed();
            }
            if (RewardType == EReward.Relic || Type == EMonster.Boss)
            {
                onDeathRelic.Raise(this);
                yield return new WaitForSeconds(0.3f);
                GameObject relicChoice = GameObject.Find("RelicChoiceUI");
                BattleStateManager.instance.BlockInputs();
                while (relicChoice.activeSelf)
                    yield return null;
                BattleStateManager.instance.OnSkillUsed();
            }

            yield return base.OnDestroyed();
        }
    }
}