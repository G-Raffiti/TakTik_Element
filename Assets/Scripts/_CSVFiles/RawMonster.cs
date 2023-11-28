using System;
using System.Collections.Generic;
using _Instances;
using _ScriptableObject;
using Relics;
using Skills._Zone;
using Stats;
using Units;
using UnityEngine;

namespace _CSVFiles
{
    public struct RawMonster
    {
        public string UnitName;
        public Element Element;
        public BattleStats BasicStats;
        public Sprite UnitSprite;
        public int Level;
        public RelicSo Relic;
        public EReward RewardType;
        public EMonster Type;
        public Archetype Archetype;
        
        public RawMonster(IReadOnlyDictionary<string, object> _csvMonster)
        {
            UnitName = _csvMonster["Name"].ToString();
            Element = UnityEngine.Resources.Load<Element>(
                $"ScriptableObject/Elements/Element_{_csvMonster["Element"]}");
            int.TryParse(_csvMonster["HP"].ToString(), out int _hp);
            int.TryParse(_csvMonster["Shield"].ToString(), out int _shield);
            int.TryParse(_csvMonster["Dodge"].ToString(), out int _dodge);
            int.TryParse(_csvMonster["Speed"].ToString(), out int _speed);
            int.TryParse(_csvMonster["Power"].ToString(), out int _power);
            int.TryParse(_csvMonster["MP"].ToString(), out int _mp);
            int.TryParse(_csvMonster["AP"].ToString(), out int _ap);
            int.TryParse(_csvMonster["Range"].ToString(), out int _range);
            int.TryParse(_csvMonster["Zone"].ToString(), out int _zone);
            int.TryParse(_csvMonster["Focus"].ToString(), out int _focus);
            BasicStats = new BattleStats
            {
                hp = _hp,
                shield = _shield,
                speed = _speed,
                power = _power,
                mp = _mp,
                ap = _ap,
                gridRange = new GridRange(EZone.Basic, EZone.Basic, _range, _zone),
            };
            UnitSprite = UnityEngine.Resources.Load<Sprite>($"Sprite/Monsters/{_csvMonster["Sprite"].ToString()}");
            int.TryParse(_csvMonster["Level"].ToString(), out Level);
            Enum.TryParse(_csvMonster["RewardType"].ToString(), out RewardType);
            Enum.TryParse(_csvMonster["Type"].ToString(), out Type);
            Enum.TryParse(_csvMonster["Archetype"].ToString(), out EArchetype _archetype);
            Archetype = UnityEngine.Resources.Load<Archetype>($"ScriptableObject/Archetypes/Archetype_{_csvMonster["Archetype"].ToString()}");
            Relic = null;
            if (RewardType == EReward.Relic || Type == EMonster.Boss)
            {
                Relic = DataBase.Relic.AllRelics.Find(_r => _r.Name == _csvMonster["Relic"].ToString());
            }
        }
    }
}