using System.Collections.Generic;
using System.Linq;
using System.Net;
using Cells;
using Skills._Zone;
using Units;
using UnityEngine;

namespace Skills.ScriptableObject_Effect
{
    [CreateAssetMenu(fileName = "SkillEffect_Curse", menuName = "Scriptable Object/Skills/Skill Effect Curse")]
    public class Curse : SkillEffect
    {
        //Todo : Curse et le Deck font le même effet d'application des Buffs => à changer !
        public override void Use(Cell _cell, SkillInfo _skillInfo)
        {
            foreach (Cell cell in Zone.GetZone(_skillInfo.Range, _cell).Where(cell => cell.CurrentUnit != null))
            {
                _skillInfo.Buffs.ForEach(_buff =>
                {
                    cell.CurrentUnit.ApplyBuff(_buff);
                });
            }
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return _cell != null;
        }

        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string str = "Apply ";
            _skillInfo.Buffs.ForEach(_buff => str += $"{_buff.Effect.Name}, ");
            str += "to the targets";
            return str;
        }
        
        public override string InfoEffect()
        {
            return "do Damage according to the Unit's Skill Power and Element Affinity";
        }
    }
}