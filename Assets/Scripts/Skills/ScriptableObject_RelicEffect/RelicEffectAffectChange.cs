using _ScriptableObject;
using Skills._Zone;
using Stats;
using UnityEngine;

namespace Skills.ScriptableObject_RelicEffect
{
    [CreateAssetMenu(fileName = "Relic_Effect_Affect_", menuName = "Scriptable Object/Relics/Relic Effect Affect Change")]
    public class RelicEffectAffectChange : RelicEffect
    {
        public override void ChangeSkill(Skill skill, RelicSO relic)
        {
            skill.ChangeAffect(relic.Affect);
        }

        public override string InfoEffect(RelicSO _relic)
        {
            return $"All Skills form this Action Pile now target {Zone.AffectToString(_relic.Affect)}";
        }
    }
}