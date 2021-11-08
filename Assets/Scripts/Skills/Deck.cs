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
        public List<SkillSO> Skills = new List<SkillSO>();
        public List<SkillSO> UsedSkills = new List<SkillSO>();
        public List<SkillSO> ConsumedSkills = new List<SkillSO>();
        public List<RelicSO> Relics = new List<RelicSO>();
        private List<IEffect> effects = new List<IEffect>();

        public List<IEffect> Effects => effects;

        public SkillSO ActualSkill { get; private set; }
        private Range range;
        public Range Range => range;
        public int Power { get; private set; }
        public List<StatusSO> StatusEffects { get; private set; }
        public Element Element { get; private set; }
        public EAffect Affect { get; private set; }
        public int Cost { get; private set; }

        private void Start()
        {
            UsedSkills = new List<SkillSO>();
            onEndBattle.EventListeners += OnBattleEndRaised;
        }

        private void OnDestroy()
        {
            onEndBattle.EventListeners -= OnBattleEndRaised;
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

            ActualSkill = Skills[0];
            UpdateActualSkill();
        }

        public void UpdateSkill(int index)
        {
            UpdateSkill(Skills[index]);
        }

        public void UpdateSkill(SkillSO _skill)
        {
            ActualSkill = _skill;
            Cost = _skill.Cost;
            effects = new List<IEffect>();
            range = new Range(_skill.Range);
            Power = _skill.Power;
            StatusEffects = new List<StatusSO>(_skill.StatusEffects);
            Element = _skill.Element;
            Affect = _skill.Affect;
            if (_skill.Effects.Count > 0)
                effects.AddRange(_skill.Effects);
            if (_skill.GridEffect != null)
                effects.Add(_skill.GridEffect);
            
            foreach (RelicSO _relic in Relics)
            {
                range += _relic.Range;
                Power += _relic.Power;
                StatusEffects.AddRange(_relic.StatusEffects);
                
                foreach (RelicEffect _relicEffect in _relic.RelicEffects)
                {
                    _relicEffect.ChangeSkill(this, _relic);   
                }
                
                if (_relic.Effect != null) effects.Add(_relic.Effect);
                if (_relic.GridEffect != null) effects.Add(_relic.GridEffect);
            }

            effects.Sort((_effect, _effect1) => _effect.ActionsOrder.CompareTo(_effect1.ActionsOrder));
        }
        
        public void UpdateActualSkill()
        {
            UpdateSkill(0);
        }

        public void Initialize()
        {
            ShuffleDeck();
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
            range.RangeType = _rangeType;
        }
        
        /// <summary>
        /// Public Method for Relics to change the Zone Type of the Skills
        /// </summary>
        public void ChangeZoneType(EZone _zoneType)
        {
            range.ZoneType = _zoneType;
        }
        
        /// <summary> 
        /// Public Method for Relics to change if the Skills of the Deck need View
        /// </summary>
        public void ChangeNeedView(bool _needView)
        {
            range.NeedView = _needView;
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
            if (effects.Any(_effect => !_effect.CanUse(_cell, _skillInfo)))
            {
                return false;
            }

            Debug.Log($"{_skillInfo.Unit} Use {_skillInfo.ColouredName()}");
            if (effects.Find(_effect => _effect is Learning) != null)
            {
                effects.Find(_effect => _effect is Learning).Use(_cell, _skillInfo);
                return true;
            }
            

            //TODO : Play Skill animation
            List<Cell> _zone = Zone.GetZone(_skillInfo.Range, _cell);
            _zone.Sort((_cell1, _cell2) =>
                _cell1.GetDistance(_skillInfo.Unit.Cell).CompareTo(_cell2.GetDistance(_skillInfo.Unit.Cell)));
            StartCoroutine(HighlightZone(_zone));
            
            foreach (IEffect _effect in effects)
            {
                _effect.Use(_cell, _skillInfo);
            }

            Skills.Remove(ActualSkill);
            if (ActualSkill.Consumable) ConsumedSkills.Add(ActualSkill);
            else UsedSkills.Add(ActualSkill);
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
        }

        public void LearnSkill(SkillSO _monsterSkill)
        {
            Skills.Remove(ActualSkill);
            ConsumedSkills.Add(ActualSkill);
            List<SkillSO> _newList = new List<SkillSO> {_monsterSkill};
            _newList.AddRange(Skills);
            Skills = new List<SkillSO>(_newList);
            NextSkill();
        }
    }
}