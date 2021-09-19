using System.Collections.Generic;
using Cells;
using Skills._Zone;
using Units;
using UnityEngine;

namespace Skills.ScriptableObject_Effect
{
    [CreateAssetMenu(fileName = "SkillEffect_LifeSteel", menuName = "Scriptable Object/Skills/Skill Effect LifeSteel")]
    public class LifeSteel : SkillEffect
    {
        [SerializeField] private float percent;
        public override void Use(Cell _cell, SkillInfo _skillInfo)
        {
            int _damage = 0;
            foreach (IEffect _effect in _skillInfo.Deck.Effects)
            {
                foreach (int _value in _effect.DamageValue(_cell,_skillInfo).Values)
                {
                    _damage += _value;
                }
            }
            
            foreach (Cell _cellAffected in Zone.GetZone(_skillInfo.Range, _cell))
            {
                Unit _unitAffected = Zone.GetUnitAffected(_cellAffected, _skillInfo);
                if (_unitAffected != null)
                {
                    _skillInfo.Unit.DefendHandler(_skillInfo.Unit, (int) (- _damage * (percent/100f)), _skillInfo.Element);
                }
            }
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return true;
        }

        public override string InfoEffect(SkillInfo _skillInfo)
        {
            if (_skillInfo.Type != ESkill.Attack || _skillInfo.Type != ESkill.Spell) return "";
            return $"Heal yourself for {percent}% of your Damage";
        }
        public override string InfoEffect()
        {
            return "heal the Unit according to the damage it do";
        }
        
        public override Dictionary<Cell, int> DamageValue(Cell _cell, SkillInfo _skillInfo)
        {
            int _damage = 0;
            foreach (IEffect _effect in _skillInfo.Deck.Effects)
            {
                foreach (int _value in _effect.DamageValue(_cell,_skillInfo).Values)
                {
                    _damage += _value;
                }
            }

            Dictionary<Cell, int> ret = new Dictionary<Cell, int>
            {
                {_skillInfo.Unit.Cell, (int) (-_damage * (percent / 100f))}
            };

            return ret;
        }
    }
}