using Skills;
using Skills._Zone;
using UnityEngine;

namespace Relics.ScriptableObject_RelicEffect
{
    [CreateAssetMenu(fileName = "Relic_Effect_Affect_", menuName = "Scriptable Object/Relics/Relic Effect Affect Change")]
    public class RelicEffectAffectChange : RelicEffect
    {
        public override void ChangeSkill(Skill skill, RelicSO relic)
        {
            skill.ChangeAffect(relic.Affect);
        }
    }
}