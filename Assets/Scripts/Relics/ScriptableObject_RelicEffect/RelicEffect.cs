using Skills;
using Units;
using UnityEngine;

namespace Relics.ScriptableObject_RelicEffect
{
    public enum ERelicEffect {ElementSwap, AffectChange, ChangeRangeType, ChangeZoneType}
    public abstract class RelicEffect : ScriptableObject
    {
        public abstract void ChangeSkill(Skill _skill, RelicSo _relic);
        public virtual void OnEndFight(Hero _hero, RelicSo _relic){}
    }
}