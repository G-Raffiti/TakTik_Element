using System;
using DataBases;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Stats
{
    [CreateAssetMenu(fileName = "Affix_", menuName = "Scriptable Object/New Affix")]
    public class AffixSo : ScriptableObject
    {
        [SerializeField] protected EAffix type;
        [SerializeField] private Color color;
        /// <summary>
        /// less is the rarity more rare is the affix
        /// </summary>
        [SerializeField] private int rarity;
        [SerializeField] private int[] tier = new int[4];
        [SerializeField] private string symbol;


        public EAffix Type => type;
        public int Rarity => rarity;
        public Color Color => color;
        public int[] Tier => tier;

        public BattleStats GenerateBs(float _value)
        {
            BattleStats _ret = new BattleStats(0);

            switch (Type)
            {
                case EAffix.Hp: _ret.hp = (int) _value;
                    break;
                case EAffix.AP: _ret.ap = (int) _value;
                    break;
                case EAffix.Mp: _ret.mp = (int) _value;
                    break;
                case EAffix.Speed: _ret.speed = (int) _value;
                    break;
                case EAffix.Shield: _ret.shield = (int) _value;
                    break;
                case EAffix.Fire: _ret.affinity.fire = _value;
                    break;
                case EAffix.Nature: _ret.affinity.nature = _value;
                    break;
                case EAffix.Water: _ret.affinity.water = _value;
                    break;
                case EAffix.Power: _ret.power = (int) _value;
                    break;
                case EAffix.Range: _ret.gridRange.rangeValue = (int) _value;
                    break;
                case EAffix.Zone: _ret.gridRange.radius = (int) _value;
                    break;
                case EAffix.Focus: _ret.focus = (int) _value;
                    break;
                default:
                    Debug.LogError("Error in Affix Type");
                    break;
            }

            return _ret;
        }
        
        
        public string Name => GetName();

        public string Icon => symbol;

        private string GetName()
        {
            return $"<color={ColorSet.HexColor(color)}>{Type}</color> ({Icon})";
                
        }
        
        public int GetValue(int _stage)
        {
            return GetValueOfTier(_stage + 1);
        }

        public int GetValueOfTier(int _tier)
        {
            int _min = tier[Math.Max(0, _tier - 1)];
            int _max = tier[Math.Min(_tier, tier.Length -1)] + 1;
            int _value = Random.Range(_min, _max);
            return _value;
        }
    }
}