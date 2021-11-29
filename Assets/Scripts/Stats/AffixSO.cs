using System;
using DataBases;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Stats
{
    [CreateAssetMenu(fileName = "Affix_", menuName = "Scriptable Object/New Affix")]
    public class AffixSO : ScriptableObject
    {
        [SerializeField] protected EAffix type;
        [SerializeField] private Color color;
        [SerializeField] private int[] tier = new int[4];
        [SerializeField] private string symbol;


        public EAffix Type => type;
        public Color Color => color;
        public int[] Tier => tier;

        public BattleStats GenerateBS(float value)
        {
            BattleStats ret = new BattleStats(0);

            switch (Type)
            {
                case EAffix.HP: ret.HP = (int) value;
                    break;
                case EAffix.AP: ret.AP = (int) value;
                    break;
                case EAffix.MP: ret.MP = (int) value;
                    break;
                case EAffix.Speed: ret.Speed = (int) value;
                    break;
                case EAffix.Shield: ret.Shield = (int) value;
                    break;
                case EAffix.Fire: ret.Affinity.Fire = value;
                    break;
                case EAffix.Nature: ret.Affinity.Nature = value;
                    break;
                case EAffix.Water: ret.Affinity.Water = value;
                    break;
                case EAffix.Power: ret.Power = (int) value;
                    break;
                case EAffix.Range: ret.Range.RangeValue = (int) value;
                    break;
                case EAffix.Zone: ret.Range.Radius = (int) value;
                    break;
                case EAffix.Focus: ret.Focus = (int) value;
                    break;
                default:
                    Debug.LogError("Error in Affix Type");
                    break;
            }

            return ret;
        }
        
        
        public string Name => GetName();

        public string Icon => symbol;

        private string GetName()
        {
            return $"<color={ColorSet.HexColor(color)}>{Type}</color> ({Icon})";
                
        }
        
        public int getValue(int lvl)
        {
            return Random.Range(tier[Math.Max(0, lvl - 1)], tier[lvl] + 1);
        }

        public float getValueOfTier(int _tier)
        {
            return Random.Range(tier[Math.Max(0, _tier - 1)], tier[_tier] + 1);
        }

        public string TierRange(int lvl)
        {
            return $"({tier[Math.Max(0, lvl - 1)]} - {tier[lvl]}) ";
        }
    }
}