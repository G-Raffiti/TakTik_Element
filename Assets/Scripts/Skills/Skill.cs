using System.Collections.Generic;
using _ScriptableObject;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Stats;
using StatusEffect;
using Units;

namespace Skills
{
    public class Skill
    {
        public Unit Unit { get; private set; }
        public Deck Deck { get; private set; }
        public Range Range { get; private set; }
        public int Power { get; private set; }
        public Element Element { get; private set; }
        public EAffect Affect { get; private set; }
        public int Cost { get; private set; }
        public ESkill Type { get; private set; }
        public List<Buff> Buffs { get; private set; }
        public List<IEffect> Effects { get; private set; }
        public SkillSO BaseSkill { get; private set; }


        public static Skill CreateSkill(SkillSO skillSO, Deck deck, Unit user)
        {
            Skill _skill = new Skill();
            _skill.Unit = user;
            _skill.Deck = deck;
            if (skillSO.Range.CanBeModified)
                _skill.Range = skillSO.Range + deck.Range + user.BattleStats.Range;
            else
                _skill.Range = skillSO.Range;
            _skill.Power = skillSO.Power + deck.Power + user.BattleStats.Power;
            _skill.Element = skillSO.Element;
            _skill.Affect = skillSO.Affect;
            _skill.Cost = skillSO.Cost + deck.Cost;
            _skill.Type = skillSO.Type;
            _skill.Buffs = new List<Buff>();
            
            foreach (StatusSO _skillSOStatus in skillSO.StatusEffects)
            {
                Buff buff = new Buff(user, _skillSOStatus);
                _skill.Buffs.Add(buff);
            }
            
            foreach (StatusSO _deckStatus in deck.StatusEffects)
            {
                Buff buff = new Buff(user, _deckStatus);
                _skill.Buffs.Add(buff);
            }

            _skill.Effects = new List<IEffect>();
            _skill.Effects.AddRange(skillSO.Effects);
            _skill.Effects.Add(skillSO.GridEffect);
            _skill.Effects.AddRange(deck.Effects);
            _skill.BaseSkill = skillSO;

            return _skill;
        }
    }
}