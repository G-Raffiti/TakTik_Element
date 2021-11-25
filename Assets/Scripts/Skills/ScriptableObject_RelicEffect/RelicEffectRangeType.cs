using _ScriptableObject;
using Skills._Zone;
using UnityEngine;

namespace Skills.ScriptableObject_RelicEffect
{
    [CreateAssetMenu(fileName = "Relic_Effect_RangeType_", menuName = "Scriptable Object/Relics/Relic Effect Range Type Change")]
    public class RelicEffectRangeType : RelicEffect
    {
        
        public override void ChangeSkill(Skill _skill, RelicSO _relic)
        {
            _skill.ChangeRangeType(_relic.Range.RangeType);
        }

        public override string InfoEffect(RelicSO _relic)
        {
            return "All Skills form this Action Pile now have a Range Type: " + _relic.Range.RangeType;
        }
    }
}