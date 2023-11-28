using System.Collections.Generic;
using _ScriptableObject;
using Buffs;
using Decks;
using Relics;
using Relics.ScriptableObject_RelicEffect;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Stats;
using Units;

namespace Skills
{
    public class Skill
    {
        public Unit Unit { get; private set; }
        public DeckMono Deck { get; private set; }
        public Relic Relic { get; private set; }
        public GridRange GridRange { get; private set; }
        public int Power { get; private set; }
        public Element Element { get; private set; }
        public EAffect Affect { get; private set; }
        public int Cost { get; private set; }
        public ESkill Type { get; private set; }
        public List<Buff> Buffs { get; private set; }
        public List<Effect> Effects { get; private set; }
        public SkillSo BaseSkill { get; private set; }


        public static Skill CreateSkill(SkillSo _skillSo, DeckMono _deck, Unit _user)
        {
            Skill _skill = new Skill();
            _skill.Deck = _deck;
            Relic _relic = _user.Relic;
            _skill.Unit = _user;
            _skill.Relic = _relic;
            if (_skillSo.GridRange.canBeModified)
                _skill.GridRange = _skillSo.GridRange + _user.battleStats.gridRange;
            else
                _skill.GridRange = _skillSo.GridRange;
            _skill.Power = _skillSo.Power + _user.battleStats.power;
            _skill.Element = _skillSo.Element;
            _skill.Affect = _skillSo.Affect;
            _skill.Cost = _skillSo.Cost;
            _skill.Type = _skillSo.Type;
            _skill.Buffs = new List<Buff>();
            
            foreach (StatusSo _skillSoStatus in _skillSo.StatusEffects)
            {
                Buff _buff = new Buff(_user, _skillSoStatus);
                _skill.Buffs.Add(_buff);
            }
            
            foreach (StatusSo _deckStatus in _relic.StatusEffects)
            {
                Buff _buff = new Buff(_user, _deckStatus);
                _skill.Buffs.Add(_buff);
            }

            _skill.Effects = new List<Effect>();

            foreach (SkillEffect _effect in _skillSo.Effects)
            {
                if (_effect.IsUnique && _skill.Effects.Contains(_effect))
                    continue;
                _skill.Effects.Add(_effect);
            }
            
            if(_skillSo.GridEffect != null)
                _skill.Effects.Add(_skillSo.GridEffect);
            
            foreach (Effect _effect in _relic.Effects)
            {
                if (_effect.IsUnique && _skill.Effects.Contains(_effect))
                    continue;
                _skill.Effects.Add(_effect);
            }

            _skill.BaseSkill = _skillSo;
            
            foreach (RelicSo _relicSo in _relic.RelicEffects)
            {
                foreach (RelicEffect _effect in _relicSo.RelicEffects)
                {
                    _effect.ChangeSkill(_skill, _relicSo);
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
                GridRange _gridRange = new GridRange(GridRange);
                _gridRange.rangeType = _rangeType;
                GridRange = _gridRange;
            }
            
            /// <summary>
            /// Public Method for Relics to change the Zone Type of the Skills
            /// </summary>
            public void ChangeZoneType(EZone _zoneType)
            {
                GridRange _gridRange = new GridRange(GridRange);
                _gridRange.zoneType = _zoneType;
                GridRange = _gridRange;
            }
            
            /// <summary> 
            /// Public Method for Relics to change if the Skills of the Deck need View
            /// </summary>
            public void ChangeNeedView(bool _needView)
            {
                GridRange _gridRange = new GridRange(GridRange);
                _gridRange.needView = _needView;
                GridRange = _gridRange;
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