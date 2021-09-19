using System;
using UnityEngine;
using UserInterface;
using Random = UnityEngine.Random;

namespace Stats
{
    [CreateAssetMenu(fileName = "Affix_", menuName = "Scriptable Object/New Affix")]
    public class AffixSO : ScriptableObject
    {
        [SerializeField] protected EAffix type;
        [SerializeField] private Color color;
        [SerializeField] private int[] tier = new int[4];

        public EAffix Type => type;
        public Color Color => color;

        public BattleStats GenerateBS(float value)
        {
            BattleStats ret = new BattleStats(0);
            
            switch (type)
            {
                case EAffix.HP:
                    ret.HP += (int) value;
                    break;
                case EAffix.AP:
                    ret.AP += (int) value;
                    break;
                case EAffix.MP:
                    ret.MP += (int) value;
                    break;
                case EAffix.Dext:
                    ret.Focus += (int) value;
                    ret.Dodge += value * 0.5f;
                    break;
                case EAffix.Strength:
                    ret.Power.Skill += value * 10;
                    ret.HP += (int)value * 5;
                    break;
                case EAffix.Intel:
                    ret.Power.Spell += value * 10;
                    ret.Shield += (int) value * 2;
                    break;
                case EAffix.Focus:
                    ret.Focus += (int) value;
                    break;
                case EAffix.Speed:
                    ret.Speed += (int)value;
                    break;
                case EAffix.Affinity:
                    ret.Power.Affinity.Fire += value;
                    ret.Power.Affinity.Nature += value;
                    ret.Power.Affinity.Water += value;
                    break;
                case EAffix.Fire:
                    ret.Power.Affinity.Fire += value;
                    break;
                case EAffix.Water:
                    ret.Power.Affinity.Water += value;
                    break;
                case EAffix.Nature:
                    ret.Power.Affinity.Nature += value;
                    break;
                case EAffix.SkillPower:
                    ret.Power.Skill += value;
                    break;
                case EAffix.SpellPower:
                    ret.Power.Spell += value;
                    break;
                case EAffix.Power:
                    ret.Power.Skill += value;
                    ret.Power.Spell += value;
                    break;
                case EAffix.Shield:
                    ret.Shield += (int)value;
                    break;
                case EAffix.Dodge:
                    ret.Dodge += value;
                    break;
                case EAffix.Range:
                    ret.Range.RangeValue += (int) value;
                    break;
                case EAffix.Zone:
                    ret.Range.Radius += (int) value;
                    break;
                case EAffix.BasicPower:
                    ret.Power.Basic += (int) value;
                    break;
                default:
                    break;
            }

            return ret;
        }
        
        
        public string Name => GetName();

        public string Icon(EAffix _affix)
        {
            switch (_affix)
            {
                case EAffix.HP:
                    return "<sprite name=HP>";
                case EAffix.AP:
                    return "<sprite name=AP>";
                case EAffix.MP:
                    return "<sprite name=MP>";
                case EAffix.Speed:
                    return "<sprite name=Speed>";
                case EAffix.Shield:
                    return "<sprite name=Shield>";
                case EAffix.Dodge:
                    return "<sprite name=Dodge>";
                case EAffix.Dext:
                    return "<sprite name=Dext>";
                case EAffix.Strength:
                    return "<sprite name=Str>";
                case EAffix.Intel:
                    return "<sprite name=Intel>";
                case EAffix.Fire:
                    return "<sprite name=Fire>";
                case EAffix.Water:
                    return "<sprite name=Water>";
                case EAffix.Nature:
                    return "<sprite name=Nature>";
                case EAffix.BasicPower:
                    return "<sprite name=BasicPower>";
                case EAffix.Focus:
                    return "<sprite name=Focus>";
                case EAffix.Power:
                    return "<sprite name=Power>";
                case EAffix.SkillPower:
                    return "<sprite name=Skill>";
                case EAffix.SpellPower:
                    return "<sprite name=Spell>";
                case EAffix.Range:
                    return "<sprite name=Range>";
                case EAffix.Zone:
                    return "<sprite name=Zone>";
                case EAffix.Affinity:
                    return "<sprite name=Affinity>";
                default:
                    return "ERROR";
            }
        }

        private string GetName()
        {
            return $"<color={ColorSet.HexColor(color)}>{Type}</color> ({Icon(Type)})";
                
        }
        
        public int getValue(int lvl)
        {
            return Random.Range(tier[Math.Max(0, lvl - 1)], tier[lvl] + 1);
        }

        public string TierRange(int lvl)
        {
            return $"({tier[Math.Max(0, lvl - 1)]} - {tier[lvl]}) ";
        }
    }
}