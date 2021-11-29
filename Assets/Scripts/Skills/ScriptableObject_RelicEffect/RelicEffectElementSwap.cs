using Relics;
using UnityEngine;

namespace Skills.ScriptableObject_RelicEffect
{
    [CreateAssetMenu(fileName = "Relic_Effect_Element_Swap_", menuName = "Scriptable Object/Relics/Relic Effect Element Swap")]
    public class RelicEffectElementSwap : RelicEffect
    {
        public override void ChangeSkill(Skill _skill, RelicSO _relic)
        {
            _skill.ChangeElement(_relic.Element);
        }

        public override string InfoEffect(RelicSO _relic)
        {
            string color = ColorUtility.ToHtmlStringRGB(_relic.Element.TextColour);
            return $"All Skills form this Action Pile are converted to <color=#{color}>{_relic.Element.Name}</color>";
        }
    }
}