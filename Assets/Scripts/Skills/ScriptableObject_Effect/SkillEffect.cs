using System.Collections.Generic;
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
    public abstract class SkillEffect : Effect
    {
        [SerializeField] private ESkill type;
        public ESkill Type => type;
        
        public override Dictionary<Cell, int> DamageValue(Cell _cell, SkillInfo _skillInfo)
        {
            Dictionary<Cell, int> _ret = new Dictionary<Cell, int>();

            return _ret;
        }
    }
}