using Skills;
using UnityEngine;

namespace Relics.ScriptableObject_RelicEffect
{
    [CreateAssetMenu(fileName = "Relic_Effect_Element_Swap_", menuName = "Scriptable Object/Relics/Relic Effect Element Swap")]
    public class RelicEffectElementSwap : RelicEffect
    {
        public override void ChangeSkill(Skill _skill, RelicSo _relic)
        {
            _skill.ChangeElement(_relic.Element);
        }
    }
}