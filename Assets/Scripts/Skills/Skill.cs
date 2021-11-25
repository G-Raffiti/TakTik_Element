using System.Collections.Generic;
using _ScriptableObject;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_RelicEffect;
using Stats;
using StatusEffect;
using Units;
using UnityEngine;

namespace Skills
{
    public class Skill
    {
        public Unit Unit { get; private set; }
        public Relic Relic { get; private set; }
        public Range Range { get; private set; }
        public int Power { get; private set; }
        public Element Element { get; private set; }
        public EAffect Affect { get; private set; }
        public int Cost { get; private set; }
        public ESkill Type { get; private set; }
        public List<Buff> Buffs { get; private set; }
        public List<IEffect> Effects { get; private set; }
        public SkillSO BaseSkill { get; private set; }


        public static Skill CreateSkill(SkillSO skillSO, Relic relic, Unit user)
        {
            Skill _skill = new Skill();
            _skill.Unit = user;
            _skill.Relic = relic;
            if (skillSO.Range.CanBeModified)
                _skill.Range = skillSO.Range + relic.Range + user.BattleStats.Range;
            else
                _skill.Range = skillSO.Range;
            _skill.Power = skillSO.Power + relic.Power + user.BattleStats.Power;
            _skill.Element = skillSO.Element;
            _skill.Affect = skillSO.Affect;
            _skill.Cost = skillSO.Cost + relic.Cost;
            _skill.Type = skillSO.Type;
            _skill.Buffs = new List<Buff>();
            
            foreach (StatusSO _skillSOStatus in skillSO.StatusEffects)
            {
                Buff buff = new Buff(user, _skillSOStatus);
                _skill.Buffs.Add(buff);
            }
            
            foreach (StatusSO _deckStatus in relic.StatusEffects)
            {
                Buff buff = new Buff(user, _deckStatus);
                _skill.Buffs.Add(buff);
            }

            _skill.Effects = new List<IEffect>();
            
            _skill.Effects.AddRange(skillSO.Effects);
            if(skillSO.GridEffect != null)
                _skill.Effects.Add(skillSO.GridEffect);
            _skill.Effects.AddRange(relic.Effects);

            Debug.Log(_skill.Effects.Count);
            
            _skill.BaseSkill = skillSO;
            
            foreach (RelicSO _relic in relic.RelicEffects)
            {
                foreach (RelicEffect _effect in _relic.RelicEffects)
                {
                    _effect.ChangeSkill(_skill, _relic);
                }
            }
            return _skill;
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
    }
    
    
}