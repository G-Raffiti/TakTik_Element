using _ScriptableObject;
using Skills._Zone;
using UnityEngine;

namespace Skills.ScriptableObject_RelicEffect
{
    [CreateAssetMenu(fileName = "Relic_Effect_NeedView_", menuName = "Scriptable Object/Relics/Relic Effect Need View Change")]
    public class RelicEffectNeedView : RelicEffect
    {
        
        public override void ChangeSkill(Deck _deck, RelicSO _relic)
        {
            _deck.ChangeNeedView(_relic.Range.NeedView);
        }

        public override string InfoEffect(RelicSO _relic)
        {
            return _relic.Range.NeedView ? "All Skills form this Action Pile now need the Line of View to be casted" : "All Skills form this Action Pile now don't need the Line of View to be casted";
        }
    }
}