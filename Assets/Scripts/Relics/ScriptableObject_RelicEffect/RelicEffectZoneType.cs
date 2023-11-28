using Skills;
using UnityEngine;

namespace Relics.ScriptableObject_RelicEffect
{
    [CreateAssetMenu(fileName = "Relic_Effect_ZoneType_", menuName = "Scriptable Object/Relics/Relic Effect Zone Type Change")]
    public class RelicEffectZoneType : RelicEffect
    {
        public override void ChangeSkill(Skill _skill, RelicSo _relic)
        {
            _skill.ChangeZoneType(_relic.BattleStats.gridRange.zoneType);
        }
    }
}