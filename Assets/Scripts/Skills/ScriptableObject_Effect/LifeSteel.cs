using System.Collections.Generic;
using System.Linq;
using _ScriptableObject;
using Cells;
using UnityEngine;

namespace Skills.ScriptableObject_Effect
{
    [CreateAssetMenu(fileName = "SkillEffect_LifeSteel", menuName = "Scriptable Object/Skills/Skill Effect LifeSteel")]
    public class LifeSteel : SkillEffect
    {
        [SerializeField] private float percent;
        public override void Use(Cell _targetCell, SkillInfo _skillInfo)
        {
            int _value = DamageValue(_targetCell, _skillInfo)[_skillInfo.unit.Cell];
            
            _skillInfo.unit.DefendHandler(_skillInfo.unit, _value, Element.None());
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return true;
        }

        public override string InfoEffect(SkillInfo _skillInfo)
        {
            if (_skillInfo.skill.Type != ESkill.Attack || _skillInfo.skill.Type != ESkill.Spell) return "";
            return $"Heal yourself for {percent}% of your Damage";
        }
        public override string InfoEffect()
        {
            return "heal the Unit according to the damage it do";
        }
        
        public override Dictionary<Cell, int> DamageValue(Cell _cell, SkillInfo _skillInfo)
        {
            int _damage = 0;
            List<Effect> _otherEffects = _skillInfo.skill.Effects.Where(_effect => !(_effect is LifeSteel)).ToList();

            foreach (Effect _effect in _otherEffects)
            {
                foreach (int _value in _effect.DamageValue(_cell,_skillInfo).Values)
                {
                    _damage += _value;
                }
            }

            Dictionary<Cell, int> _ret = new Dictionary<Cell, int>
            {
                {_skillInfo.unit.Cell, (int) (-_damage * (percent / 100f))}
            };

            return _ret;
        }
    }
}