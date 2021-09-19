using System;
using System.Collections.Generic;
using Stats;
using Units;
using UnityEngine;

namespace Gears
{
    [Serializable]
    public class Inventory
    {
        public List<Gear> gears = new List<Gear>();

        public Inventory() {}
        public Inventory(SaveInventory _save)
        {
            gears = new List<Gear>(_save.gears);
        }

        public void GenerateLootGear(int lvl)
        {
            Gear gear = new Gear();
            gear.CreateGear(lvl);
            gears.Add(gear);
        }
        
        public void GenerateGearFor(MonsterSO monster, int Stage)
        {
            //TODO : create archetype of monster like Mage, Warrior, Rogue and add a List of Gear tha can't be used for each archetype then in CreateGear take random but the List (in MonsterSO do a EArchetype)
            Gear gear = new Gear();
            gear.CreateGear(Stage);
            gears.Add(gear);
        }

        public BattleStats GearStats()
        {
            BattleStats ret = new BattleStats(0);
            gears.ForEach(gear => ret += gear.GetStats());
            return ret;
        }
    }

    [Serializable]
    public class SaveInventory
    {
        public List<Gear> gears;

        public SaveInventory(Inventory _toSave)
        {
            gears = new List<Gear>(_toSave.gears);
        }
    }
}