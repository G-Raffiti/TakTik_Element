using Skills;
using Units;

namespace Relics.ScriptableObject_RelicEffect
{
    public class RelicEffectHealOnEndFight : RelicEffect
    {
        public override void ChangeSkill(Skill _skill, RelicSo _relic)
        {
        }

        public override void OnEndFight(Hero _hero, RelicSo _relic)
        {
            _hero.HealFixValueHp((int)_relic.EffectFactor);
        }
    }
}