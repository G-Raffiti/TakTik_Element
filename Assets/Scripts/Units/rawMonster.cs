using System;
using System.Collections.Generic;
using _Instances;
using _ScriptableObject;
using Skills;
using Skills._Zone;
using Stats;
using UnityEngine;

namespace Units
{
    public struct rawMonster
    {
        public string unitName;
        public Element element;

        public BattleStats basicStats;

        public Sprite unitSprite;

        public int level;
        
        public RelicSO relic;
        public EReward rewardType;
        public EMonster type;
        public Archetype archetype;
        
        public rawMonster(Dictionary<string, object> csvMonster)
        {
            unitName = csvMonster["Name"].ToString();
            element = UnityEngine.Resources.Load<Element>(
                $"ScriptableObject/Elements/Element_{csvMonster["Element"]}");
            int.TryParse(csvMonster["HP"].ToString(), out int hp);
            int.TryParse(csvMonster["Shield"].ToString(), out int shield);
            int.TryParse(csvMonster["Dodge"].ToString(), out int dodge);
            int.TryParse(csvMonster["Speed"].ToString(), out int speed);
            int.TryParse(csvMonster["Power"].ToString(), out int power);
            int.TryParse(csvMonster["MP"].ToString(), out int mp);
            int.TryParse(csvMonster["AP"].ToString(), out int ap);
            int.TryParse(csvMonster["Range"].ToString(), out int range);
            int.TryParse(csvMonster["Zone"].ToString(), out int zone);
            int.TryParse(csvMonster["Focus"].ToString(), out int focus);
            basicStats = new BattleStats
            {
                HP = hp,
                Shield = shield,
                Speed = speed,
                Power = power,
                MP = mp,
                AP = ap,
                Range = new Range(EZone.Basic, EZone.Basic, range, zone),
            };
            unitSprite = UnityEngine.Resources.Load<Sprite>($"Sprite/Monsters/{csvMonster["Sprite"].ToString()}");
            int.TryParse(csvMonster["Level"].ToString(), out level);
            Enum.TryParse(csvMonster["RewardType"].ToString(), out rewardType);
            Enum.TryParse(csvMonster["Type"].ToString(), out type);
            Enum.TryParse(csvMonster["Archetype"].ToString(), out EArchetype _archetype);
            archetype = UnityEngine.Resources.Load<Archetype>($"ScriptableObject/Archetypes/Archetype_{csvMonster["Archetype"].ToString()}");
            relic = null;
            if (rewardType == EReward.Relic || type == EMonster.Boss)
            {
                relic = DataBase.Relic.AllRelics.Find(r => r.Name == csvMonster["Relic"].ToString());
            }
        }
    }
}