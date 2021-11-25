using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _EventSystem.Listeners;
using _Extension;
using _Instances;
using _ScriptableObject;
using Cells;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_RelicEffect;
using Stats;
using StatusEffect;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Skills
{
    public class Deck : MonoBehaviour
    {
        [SerializeField] private BoolEvent onEndBattle;
        
        public List<SkillSO> Hand = new List<SkillSO>();
        public List<SkillSO> Skills = new List<SkillSO>();
        public List<SkillSO> UsedSkills = new List<SkillSO>();
        public List<SkillSO> ConsumedSkills = new List<SkillSO>();
        
        public List<RelicSO> Relics = new List<RelicSO>();

        public List<Skill> UsableSkills = new List<Skill>();
        public Skill ActualSkill { get; private set; }
        public List<IEffect> Effects { get; private set; }
        public Range Range { get; private set; }
        public int Power { get; private set; }
        public List<StatusSO> StatusEffects { get; private set; }
        public Element Element { get; private set; }
        public EAffect Affect { get; private set; }
        public int Cost { get; private set; }

        private void Start()
        {
            UsedSkills = new List<SkillSO>();
            UpdateDeck();
            onEndBattle.EventListeners += OnBattleEndRaised;
        }

        private void OnDestroy()
        {
            onEndBattle.EventListeners -= OnBattleEndRaised;
        }

        /// <summary>
        /// Add up all the relics
        /// </summary>
        public void UpdateDeck()
        {
            Range = new Range(0);
            Power = 0;
            Cost = 0;
            StatusEffects = new List<StatusSO>();
            Effects = new List<IEffect>();
            
            foreach (RelicSO _relic in Relics)
            {
                Cost += _relic.Cost;
                Range += _relic.Range;
                Power += _relic.Power;
                StatusEffects.AddRange(_relic.StatusEffects);
                
                foreach (RelicEffect _relicEffect in _relic.RelicEffects)
                {
                    _relicEffect.ChangeSkill(this, _relic);   
                }
                
                if (_relic.Effect != null) Effects.Add(_relic.Effect);
                if (_relic.GridEffect != null) Effects.Add(_relic.GridEffect);
            }
        }

        /// <summary>
        /// Shuffle the Deck and the Discard together and change the Actual Skill to be the first of the shuffled list.
        /// </summary>
        public void ShuffleDeck()
        {
            Skills.AddRange(UsedSkills);
            UsedSkills = new List<SkillSO>();

            if (Skills.Count < 1)
            {
                Skills.Add(DataBase.Skill.Learning);
            }
            
            Skills.Shuffle();
            NextSkill();
        }

        /// <summary>
        /// Method Called when a Skill is Used to find the next one.
        /// </summary>
        public void NextSkill()
        {
            if (Skills.Count == 0)
            {
                ShuffleDeck();
            }
            ActualSkill = UsableSkills[0];
        }
        
    #region Change Methode for Relics

        /// <summary>
        /// Public Method for the Relics to change the Element of the Skills
        /// </summary>
        public void ChangeElement(Element _element)
        {
            Element = _element;
        }
        
        /// <summary>
        /// Public Method for Relics to change witch Units can be affected by the Skills
        /// </summary>
        public void ChangeAffect(EAffect _affect)
        {
            Affect = _affect;
        }
        
        /// <summary>
        /// Public Method for Relics to change the Range Type of the Skills
        /// </summary>
        public void ChangeRangeType(EZone _rangeType)
        {
            Range range = new Range(Range);
            range.RangeType = _rangeType;
            Range = range;
        }
        
        /// <summary>
        /// Public Method for Relics to change the Zone Type of the Skills
        /// </summary>
        public void ChangeZoneType(EZone _zoneType)
        {
            Range range = new Range(Range);
            range.ZoneType = _zoneType;
            Range = range;
        }
        
        /// <summary> 
        /// Public Method for Relics to change if the Skills of the Deck need View
        /// </summary>
        public void ChangeNeedView(bool _needView)
        {
            Range range = new Range(Range);
            range.NeedView = _needView;
            Range = range;
        }
        
        /// <summary> 
        /// Public Method for Relics to change the Skill's Cost
        /// </summary>
        public void ChangeCost(int _added)
        {
            Cost += _added;
            if (Cost < 0)
                Cost = 0;
        }

    #endregion

        /// <summary>
        /// Methode Called on Skill Use, it will trigger all Skill Effects and Grid Effects on the Affected Cells
        /// </summary>
        /// <param name="_cell">Cell Clicked</param>
        /// <returns></returns>
        public bool UseSkill(SkillInfo _skillInfo, Cell _cell)
        {
            if (_skillInfo.Skill.Effects.Any(_effect => !_effect.CanUse(_cell, _skillInfo)))
            {
                return false;
            }

            Debug.Log($"{_skillInfo.Skill.Unit} Use {_skillInfo.ColouredName()}");
            if (_skillInfo.Skill.Effects.Find(_effect => _effect is Learning) != null)
            {
                _skillInfo.Skill.Effects.Find(_effect => _effect is Learning).Use(_cell, _skillInfo);
                return true;
            }
            

            //TODO : Play Skill animation
            List<Cell> _zone = Zone.GetZone(_skillInfo.Skill.Range, _cell);
            _zone.Sort((_cell1, _cell2) =>
                _cell1.GetDistance(_skillInfo.Skill.Unit.Cell).CompareTo(_cell2.GetDistance(_skillInfo.Skill.Unit.Cell)));
            StartCoroutine(HighlightZone(_zone));
            
            foreach (IEffect _effect in _skillInfo.Skill.Effects)
            {
                _effect.Use(_cell, _skillInfo);
            }

            Hand.Remove(ActualSkill.BaseSkill);
            if (ActualSkill.BaseSkill.Consumable) ConsumedSkills.Add(ActualSkill.BaseSkill);
            else UsedSkills.Add(ActualSkill.BaseSkill);
            NextSkill();
            
            return true;
        }

        /// <summary>
        /// Method Called on Monster Spawn to initialize the Info Skill they Use.
        /// </summary>
        /// <param name="_skillSO"></param>
        /// <param name="_relicSO"></param>
        public void InitializeForMonster(SkillSO _skillSO, List<RelicSO> _relicSO)
        {
            if (_skillSO != null)
                Skills.Add(_skillSO);
            if (_relicSO != null)
                Relics.AddRange(_relicSO);
            NextSkill();
        }

        private static IEnumerator HighlightZone(List<Cell> zone)
        {
            foreach (Cell _cell in zone)
            {
                _cell.MarkAsHighlighted();
                yield return new WaitForSeconds(0.05f);
            }
            foreach (Cell _cell in zone)
            {
                _cell.MarkAsHighlighted();
            }
            yield return new WaitForSeconds(0.2f);
            zone.ForEach(c => c.UnMark());
        }

        private void OnBattleEndRaised(bool item)
        {
            Skills.AddRange(ConsumedSkills);
            ConsumedSkills = new List<SkillSO>();
            ShuffleDeck();
            Skills.Sort((s, s2) => String.Compare(s.Name, s2.Name, StringComparison.Ordinal));
            Skills.Sort((s, s2) => s.Cost.CompareTo(s2.Cost));
        }

        public void LearnSkill(SkillSO _monsterSkill)
        {
            Skills.Remove(ActualSkill.BaseSkill);
            ConsumedSkills.Add(ActualSkill.BaseSkill);
            List<SkillSO> _newList = new List<SkillSO> {_monsterSkill};
            _newList.AddRange(Skills);
            Skills = new List<SkillSO>(_newList);
            NextSkill();
        }

        /// <summary>
        /// Used in Camp to Swap Skill or in Event to Add a Skill to a Specific Deck
        /// </summary>
        /// <param name="LearnSkill">Skill to Add in the Deck</param>
        public void AddSkill(SkillSO LearnSkill)
        {
            Skills.Add(LearnSkill);
        }
        
        /// <summary>
        /// Used in Camp to Swap Skill or in Event to Forget a Skill Completly
        /// </summary>
        /// <param name="_Skill"></param>
        /// <returns></returns>
        public bool RemoveSkill(SkillSO _Skill)
        {
            if (!Skills.Contains(_Skill)) return false;
            Skills.Remove(_Skill);
            return true;
        }
    }
}