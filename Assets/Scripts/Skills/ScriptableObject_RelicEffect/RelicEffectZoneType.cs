using Relics;
using UnityEngine;

namespace Skills.ScriptableObject_RelicEffect
{
    [CreateAssetMenu(fileName = "Relic_Effect_ZoneType_", menuName = "Scriptable Object/Relics/Relic Effect Zone Type Change")]
    public class RelicEffectZoneType : RelicEffect
    {
        public override void ChangeSkill(Skill _skill, RelicSO _relic)
        {
            _skill.ChangeZoneType(_relic.Range.ZoneType);
        }

        public override string InfoEffect(RelicSO _relic)
        {
            return "All Skills form this Action Pile now have a Zone Type: " + _relic.Range.ZoneType;
        }
    }
}