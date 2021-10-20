using System.Collections.Generic;
using Cells;
using UnityEngine;

namespace Skills.ScriptableObject_GridEffect
{
    public enum EGridEffect
    {
        None,
        Push,
        MoveTo,
        Teleport,
        TeleportTo,
        Pull,
        GoAwayFrom,
        PushImpact,
    }
    
    public abstract class SkillGridEffect : IEffect
    {
        [SerializeField] private EGridEffect type;
        public EGridEffect Type => type;


        public override Dictionary<Cell, int> DamageValue(Cell _cell, SkillInfo _skillInfo)
        {
            Dictionary<Cell, int> ret = new Dictionary<Cell, int>();

            return ret;
        }
    }
    
}