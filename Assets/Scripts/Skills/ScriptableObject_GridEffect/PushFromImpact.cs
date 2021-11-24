using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Instances;
using _ScriptableObject;
using Cells;
using Grid;
using Pathfinding.Algorithms;
using Skills._Zone;
using Stats;
using Units;
using UnityEngine;

namespace Skills.ScriptableObject_GridEffect
{
    [CreateAssetMenu(fileName = "GridEffect_PushImpact", menuName = "Scriptable Object/Skills/Grid Effect Push Impact")]
    public class PushFromImpact : SkillGridEffect
    {
        public override void Use(Cell _cell, SkillInfo _skillInfo)
        {
            List<Cell> _zone = Zone.GetZone(_skillInfo.Range, _cell);

            List<IMovable> affecteds = new List<IMovable>();
            foreach (Cell _cellAffected in _zone)
            {
                if (Zone.GetAffected(_cellAffected, _skillInfo) != null)
                    affecteds.Add(Zone.GetAffected(_cellAffected, _skillInfo));
            }

            if (affecteds.Count == 0) return;
            
            DataBase.RunCoroutine(PushFix.Push(affecteds, _cell, _skillInfo, (int)Math.Max(1, _skillInfo.Unit.BattleStats.GetPower(_skillInfo.Element.Type)/5f)));
        }

        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string str = $"<sprite name=Push> {(int)Math.Max(1, _skillInfo.Unit.BattleStats.GetPower(_skillInfo.Element.Type)/5f)} Away from the Skill's Impact Cell";
            return str;
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