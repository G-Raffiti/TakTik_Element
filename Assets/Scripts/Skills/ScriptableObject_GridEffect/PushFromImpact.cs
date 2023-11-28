using System;
using System.Collections.Generic;
using _Instances;
using Cells;
using Skills._Zone;
using Units;
using UnityEngine;

namespace Skills.ScriptableObject_GridEffect
{
    [CreateAssetMenu(fileName = "GridEffect_PushImpact", menuName = "Scriptable Object/Skills/Grid Effect Push Impact")]
    public class PushFromImpact : SkillGridEffect
    {
        public override void Use(Cell _targetCell, SkillInfo _skillInfo)
        {
            List<Cell> _zone = Zone.GetZone(_skillInfo.skill.GridRange, _targetCell);

            List<Movable> _affecteds = new List<Movable>();
            foreach (Cell _cellAffected in _zone)
            {
                if (Zone.GetAffected(_cellAffected, _skillInfo.skill) != null)
                    _affecteds.Add(Zone.GetAffected(_cellAffected, _skillInfo.skill));
            }

            if (_affecteds.Count == 0) return;
            
            Utility.RunCoroutine(PushFix.Push(_affecteds, _targetCell, _skillInfo, (int)Math.Max(1, _skillInfo.unit.battleStats.GetPower(_skillInfo.skill.Element.Type)/5f)));
        }

        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string _str = $"<sprite name=Push> {(int)Math.Max(1, _skillInfo.unit.battleStats.GetPower(_skillInfo.skill.Element.Type)/5f)} Away from the Skill's Impact Cell";
            return _str;
        }

        public override string InfoEffect()
        {
            return "Push All Targets Away from the Skill's Impact Cell ";
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return true;
        }
    }
}