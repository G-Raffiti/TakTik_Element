using _CSVFiles;
using _Instances;
using _ScriptableObject;
using Cells;
using Relics;
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

    public enum EArchetype
    {
        None,
        Warrior,
        Caster,
        Ranger,
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
        [SerializeField] private Archetype archetype;
        [SerializeField] private SkillSO skill;
        
        public SkillSO Skill => skill;
        public RelicSO Relic => relic;
        public EReward RewardType => rewardType;
        public EMonster Type => type;

        private const float GoodAffinity = 3;
        private const float BadAffinity = 5;
        private const int SkillRewardBonusAP = 1;

        /// <summary>
        /// create Stats between adding the basic Stats to a Randomize Stats influenced by the Current Stage of the game;
        /// set the Affinity of a monster to 75 / 100 / 150
        /// </summary>
        public BattleStats Stats()
        {
            BattleStats _ret = new BattleStats(basicStats);
            
            _ret.Affinity = AffinityFromElement(element, _ret.Power);
            
            if (RewardType == EReward.Skill)
                _ret.AP += SkillRewardBonusAP;

            _ret.Power = 0;

            _ret.HP *= (KeepBetweenScene.Stage + 1);
            _ret.Shield *= (KeepBetweenScene.Stage + 1);
            
            return _ret;
        }

        private Affinity AffinityFromElement(Element ele, int basePower)
        {
            float fire = 0;
            float nat = 0;
            float wat = 0;
            switch (ele.Type)
            {
                case EElement.Fire: 
                    fire += GoodAffinity + basePower;
                    nat += BadAffinity;
                    wat -= BadAffinity;
                    break;
                case EElement.Nature:
                    nat += GoodAffinity + basePower;
                    wat += BadAffinity;
                    fire -= BadAffinity;
                    break;
                case EElement.Water:
                    wat += GoodAffinity + basePower;
                    fire += BadAffinity;
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
        public Archetype Archetype => archetype;

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

        public void SetDATA(RawMonster _rawMonster)
        {
            unitName = _rawMonster.UnitName;
            element = _rawMonster.Element;
            basicStats = _rawMonster.BasicStats;
            unitSprite = _rawMonster.UnitSprite;
            level = _rawMonster.Level;
            relic = _rawMonster.Relic;
            rewardType = _rawMonster.RewardType;
            type = _rawMonster.Type;
            archetype = _rawMonster.Archetype;
            prefab = UnityEngine.Resources.Load<GameObject>($"Prefabs/Units/PrefabMonster");
        }
    }
}