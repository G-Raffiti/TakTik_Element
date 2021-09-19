using System;
using _Instances;
using _ScriptableObject;
using Cells;
using Grid;
using Skills;
using Stats;
using UnityEngine;

namespace Units
{

    public enum EReward
    {
        None, Gear, Skill, Relic,
    }

    public enum EMonster
    {
        Minion, Boss, Invoc,
    }
    
    [CreateAssetMenu(fileName = "Monster_", menuName = "Scriptable Object/New Monster")]
    public class MonsterSO : ScriptableObject
    {
        [SerializeField] private string unitName;
        [SerializeField] private Element element;

        [SerializeField] private BattleStats basicStats;
        [SerializeField] private BattleStats randomStats;

        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite unitSprite;

        [SerializeField] private int level;
        
        [SerializeField] private RelicSO relic;
        [SerializeField] private EReward rewardType;
        [SerializeField] private EMonster type;
        [SerializeField] private SkillSO skill;
        
        public SkillSO Skill => skill;
        public RelicSO Relic => relic;
        public EReward RewardType => rewardType;
        public EMonster Type => type;

        private const float GoodAffinity = 50;
        private const float BadAffinity = 25;
        private const int SkillRewardBonusAP = 1;

        /// <summary>
        /// create Stats between adding the basic Stats to a Randomize Stats influenced by the Current Stage of the game;
        /// set the Affinity of a monster to 75 / 100 / 150
        /// </summary>
        public BattleStats Stats()
        {
            BattleStats random = BattleStats.Randomize(randomStats * Math.Max(KeepBetweenScene.Stage - 1, 0), randomStats * KeepBetweenScene.Stage);
            random.Power.SetAffinity(AffinityFromElement(element));
            if (RewardType == EReward.Skill)
                random.AP += SkillRewardBonusAP;

            return basicStats + random;
        }

        private Affinity AffinityFromElement(Element ele)
        {
            float fire = 100;
            float nat = 100;
            float wat = 100;
            switch (ele.Type)
            {
                case EElement.Fire: 
                    fire += GoodAffinity;
                    wat -= BadAffinity;
                    break;
                case EElement.Nature:
                    nat += GoodAffinity;
                    fire -= BadAffinity;
                    break;
                case EElement.Water:
                    wat += GoodAffinity;
                    nat -= BadAffinity;
                    break;
                case EElement.None:
                    break;
                default:
                    break;
            }

            return new Affinity(fire, nat, wat);
        }

        public string Name => unitName;
        public int Level => level;
        public Sprite UnitSprite => unitSprite;
        public Element Element => element;
        
        /// <summary>
        /// Create the Prefab with the Stats at the Cell Location
        /// </summary>
        /// <param name="_cell">where the Prefab is created</param>
        public void Spawn(Cell _cell)
        {
            GameObject _pref = Instantiate(prefab, GameObject.Find("Units").transform);
            _pref.transform.position = _cell.transform.position;
            Monster _monster = _pref.GetComponent<Monster>();
            if (_monster != null)
            {
                _monster.Spawn(this, KeepBetweenScene.Stage);
            }

            _cell.Take(_monster);
        }
    }
}