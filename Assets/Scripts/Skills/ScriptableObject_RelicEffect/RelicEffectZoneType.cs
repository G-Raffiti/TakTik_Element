using _ScriptableObject;
using Skills._Zone;
using UnityEngine;

namespace Skills.ScriptableObject_RelicEffect
{
    [CreateAssetMenu(fileName = "Relic_Effect_ZoneType_", menuName = "Scriptable Object/Relics/Relic Effect Zone Type Change")]
    public class RelicEffectZoneType : RelicEffect
    {
        public override void ChangeSkill(Deck _deck, RelicSO _relic)
        {
            _deck.ChangeZoneType(_relic.Range.ZoneType);
        }

        public override string InfoEffect(RelicSO _relic)
        {
            return "All Skills form this Action Pile now have a Zone Type: " + _relic.Range.ZoneType;
        }
    }
}