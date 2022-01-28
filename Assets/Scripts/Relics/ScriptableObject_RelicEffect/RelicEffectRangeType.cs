using Skills;
using UnityEngine;

namespace Relics.ScriptableObject_RelicEffect
{
    [CreateAssetMenu(fileName = "Relic_Effect_RangeType_", menuName = "Scriptable Object/Relics/Relic Effect Range Type Change")]
    public class RelicEffectRangeType : RelicEffect
    {
        
        public override void ChangeSkill(Skill _skill, RelicSO _relic)
        {
            _skill.ChangeRangeType(_relic.BattleStats.Range.RangeType);
        }
    }
}