using _ScriptableObject;
using Skills._Zone;
using UnityEngine;

namespace Skills.ScriptableObject_RelicEffect
{
    [CreateAssetMenu(fileName = "Relic_Effect_Cost_", menuName = "Scriptable Object/Relics/Relic Effect Cost")]
    public class Cost : RelicEffect
    {
        [SerializeField] private int value;
        public override void ChangeSkill(Deck _deck, RelicSO _relic)
        {
            _deck.ChangeCost(value);
        }

        public override string InfoEffect(RelicSO _relic)
        {
            return $"All Skills form this Action Pile now target {Zone.AffectToString(_relic.Affect)}";
        }
    }
}