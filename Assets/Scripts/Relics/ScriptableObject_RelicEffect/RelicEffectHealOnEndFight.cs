using Skills;
using Units;

namespace Relics.ScriptableObject_RelicEffect
{
    public class RelicEffectHealOnEndFight : RelicEffect
    {
        public override void ChangeSkill(Skill skill, RelicSO relic)
        {
        }

        public override void OnEndFight(Hero hero, RelicSO relic)
        {
            hero.HealFixValueHP((int)relic.EffectFactor);
        }
    }
}