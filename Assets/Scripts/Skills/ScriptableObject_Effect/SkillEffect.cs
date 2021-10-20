﻿using System.Collections.Generic;
using Cells;
using UnityEngine;

namespace Skills.ScriptableObject_Effect
{
    public enum ESkill
    {
        None = 100,
        Attack,
        Medicine,
        Spell,
        Healing,
        Buff,
        LifeSteel,
        Learning,
        FloorBuff,
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