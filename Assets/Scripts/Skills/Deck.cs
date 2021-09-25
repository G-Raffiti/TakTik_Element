using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _EventSystem.Listeners;
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
    public class Deck : MonoBehaviour, IGameEventListener<bool>
    {
        [SerializeField] private BoolEvent onEndBattle;
        public List<SkillSO> Skills = new List<SkillSO>();
        public List<SkillSO> ConsumedSkills = new List<SkillSO>();
        public List<SkillSO> UsedSkills { get; set; }
        public List<RelicSO> Relics = new List<RelicSO>();
        private List<IEffect> effects = new List<IEffect>();

        public List<IEffect> Effects => effects;

        public SkillSO ActualSkill { get; private set; }
        private Range range;
        public Range Range => range;
        public Power Power { get; private set; }
        public List<StatusSO> StatusEffects { get; private set; }
        public Element Element { get; private set; }
        public EAffect Affect { get; private set; }

        private void Start()
        {
            UsedSkills = new List<SkillSO>();
            onEndBattle.RegisterListener(this);
        }

        /// <summary>
        /// Shuffle the Deck and change the Actual Skill to an other one
        /// All Skills can be selected as the new one except the one that has been used
        /// </summary>
        public void ShuffleDeck()
        {
            if (Skills.Count < 1)
            {
                Skills.AddRange(UsedSkills);
                UsedSkills = new List<SkillSO>();
            }
            List<SkillSO> _list = Skills.Count == 1 ? Skills : Skills.Where(_skill => _skill != ActualSkill).ToList();
            ActualSkill = _list[Random.Range(0, _list.Count)];

            UpdateSkill();
        }

        public void UpdateSkill()
        {
            effects = new List<IEffect>();
            range = ActualSkill.Range;
            Power = ActualSkill.Power;
            StatusEffects = ActualSkill.StatusEffects;
            Element = ActualSkill.Element;
            Affect = ActualSkill.Affect;
            if (ActualSkill.Effect != null)
                effects.Add(ActualSkill.Effect);
            if (ActualSkill.GridEffect != null)
                effects.Add(ActualSkill.GridEffect);
            
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
                Skills.Remove(ActualSkill);
                ConsumedSkills.Add(ActualSkill);
                return true;
            }
            
            List<Cell> _zone = Zone.GetZone(_skillInfo.Range, _cell);
            _zone.Sort((_cell1, _cell2) =>
                _cell1.GetDistance(_skillInfo.Unit.Cell).CompareTo(_cell2.GetDistance(_skillInfo.Unit.Cell)));
            
            //TODO : Play Skill animation
            StartCoroutine(HighlightZone(_zone));
            
            foreach (IEffect _effect in effects)
            {
                _effect.Use(_cell, _skillInfo);
            }
            
            foreach (Buff _buff in _skillInfo.Buffs)
            {
                foreach (Cell _cellAffected in _zone)
                {
                    Unit _unitAffected = Zone.GetUnitAffected(_cellAffected, _skillInfo);
                    if (_unitAffected != null)
                        _unitAffected.ApplyBuff(_buff);
                }
            }

            Skills.Remove(ActualSkill);
            if (ActualSkill.Consumable) ConsumedSkills.Add(ActualSkill);
            else UsedSkills.Add(ActualSkill);
            
            return true;
        }

        public void Initialize(SkillSO _skillSO, List<RelicSO> _relicSO)
        {
            if (_skillSO != null)
                Skills.Add(_skillSO);
            if (_relicSO != null)
                Relics.AddRange(_relicSO);
            ShuffleDeck();
        }

        public static IEnumerator HighlightZone(List<Cell> zone)
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

        public void OnEventRaised(bool item)
        {
            Skills.AddRange(ConsumedSkills);
            ConsumedSkills = new List<SkillSO>();
        }

        public void LastSkill()
        {
            ActualSkill = Skills[Skills.Count - 1];

            UpdateSkill();
        }
    }
}