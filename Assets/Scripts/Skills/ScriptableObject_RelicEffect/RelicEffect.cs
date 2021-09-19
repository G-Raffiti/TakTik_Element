﻿using UnityEngine;

namespace Skills.ScriptableObject_RelicEffect
{
    public enum ERelicEffect {ElementSwap, AffectChange, ChangeRangeType, ChangeZoneType}
    public abstract class RelicEffect : ScriptableObject
    {
        public abstract void ChangeSkill(Deck _deck, RelicSO _relic);
        public abstract string InfoEffect(RelicSO _relic);
    }
}