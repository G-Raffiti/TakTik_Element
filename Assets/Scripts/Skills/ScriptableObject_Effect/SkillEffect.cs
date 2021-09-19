using System;
using System.Collections.Generic;
using _ScriptableObject;
using Cells;
using Grid;
using Units;
using UnityEngine;

namespace Skills.ScriptableObject_Effect
{
    public enum ESkill
    {
        Attack,
        Medicine,
        Spell,
        Healing,
        Buff,
        Curse,
        LifeSteel,
        Learning,
    }
    public abstract class SkillEffect : IEffect
    {
        [SerializeField] private ESkill type;
        public ESkill Type => type;
        public override Dictionary<Cell, int> DamageValue(Cell _cell, SkillInfo _skillInfo)
        {
            Dictionary<Cell, int> ret = new Dictionary<Cell, int>();

            return ret;
        }
    }
}