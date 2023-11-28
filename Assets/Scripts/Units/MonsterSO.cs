using _CSVFiles;
using _Instances;
using _ScriptableObject;
using Cells;
using Relics;
using Skills;
using Stats;
using UnityEditor;
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
    public class MonsterSo : ScriptableObject
    {
        [SerializeField] private string unitName;
        [SerializeField] private Element element;

        [SerializeField] private BattleStats basicStats;

        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite unitSprite;

        [SerializeField] private RelicSo relic;
        [SerializeField] private EReward rewardType;
        [SerializeField] private EMonster type;
        [SerializeField] private Archetype archetype;
        [SerializeField] private SkillSo skill;
        
        public SkillSo Skill => skill;
        public RelicSo Relic => relic;
        public EReward RewardType => rewardType;
        public EMonster Type => type;

        private const float GOOD_AFFINITY = 3;
        private const float BAD_AFFINITY = 5;
        private const int SKILL_REWARD_BONUS_AP = 1;

        /// <summary>
        /// create Stats between adding the basic Stats to a Randomize Stats influenced by the Current Stage of the game;
        /// set the Affinity of a monster to 75 / 100 / 150
        /// </summary>
        public BattleStats Stats()
        {
            BattleStats _ret = new BattleStats(basicStats);
            
            _ret.affinity = AffinityFromElement(element, _ret.power);
            
            if (RewardType == EReward.Skill)
                _ret.ap += SKILL_REWARD_BONUS_AP;

            _ret.power = 0;

            _ret.hp *= (BattleStage.Stage + 1);
            _ret.shield *= (BattleStage.Stage + 1);
            
            return _ret;
        }

        private Affinity AffinityFromElement(Element _ele, int _basePower)
        {
            float _fire = 0;
            float _nat = 0;
            float _wat = 0;
            switch (_ele.Type)
            {
                case EElement.Fire: 
                    _fire += GOOD_AFFINITY + _basePower;
                    _nat += BAD_AFFINITY;
                    _wat -= BAD_AFFINITY;
                    break;
                case EElement.Nature:
                    _nat += GOOD_AFFINITY + _basePower;
                    _wat += BAD_AFFINITY;
                    _fire -= BAD_AFFINITY;
                    break;
                case EElement.Water:
                    _wat += GOOD_AFFINITY + _basePower;
                    _fire += BAD_AFFINITY;
                    _nat -= BAD_AFFINITY;
                    break;
                case EElement.None:
                    break;
                default:
                    break;
            }

            return new Affinity(_fire, _nat, _wat);
        }

        public string Name => unitName;
        public int Level => Type == EMonster.Minion ? 1 : 3;
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
                _monster.Spawn(this, BattleStage.Stage);
            }
            _cell.ForceTake(_monster);
        }

        public void SetData(RawMonster _rawMonster)
        {
            unitName = _rawMonster.UnitName;
            element = _rawMonster.Element;
            basicStats = _rawMonster.BasicStats;
            unitSprite = _rawMonster.UnitSprite;
            relic = _rawMonster.Relic;
            rewardType = _rawMonster.RewardType;
            type = _rawMonster.Type;
            archetype = _rawMonster.Archetype;
            prefab = UnityEngine.Resources.Load<GameObject>($"Prefabs/Units/PrefabMonster");
        }

        public void SetSkill(SkillSo _newSkill)
        {
#if (UNITY_EDITOR)
            skill = _newSkill;
            EditorUtility.SetDirty(this); 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }
}