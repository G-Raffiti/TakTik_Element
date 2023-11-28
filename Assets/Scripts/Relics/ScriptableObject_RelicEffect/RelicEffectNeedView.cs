using Skills;
using UnityEngine;

namespace Relics.ScriptableObject_RelicEffect
{
    [CreateAssetMenu(fileName = "Relic_Effect_NeedView_", menuName = "Scriptable Object/Relics/Relic Effect Need View Change")]
    public class RelicEffectNeedView : RelicEffect
    {
        
        public override void ChangeSkill(Skill _skill, RelicSO _relic)
        {
            _skill.ChangeNeedView(_relic.BattleStats.gridRange.NeedView);
        }
    }
}