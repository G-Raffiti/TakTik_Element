using Skills;
using Units;
using UnityEngine;

namespace Relics.ScriptableObject_RelicEffect
{
    public enum ERelicEffect {ElementSwap, AffectChange, ChangeRangeType, ChangeZoneType}
    public abstract class RelicEffect : ScriptableObject
    {
        public abstract void ChangeSkill(Skill skill, RelicSO relic);
        public virtual void OnEndFight(Hero hero, RelicSO relic){}
    }
}