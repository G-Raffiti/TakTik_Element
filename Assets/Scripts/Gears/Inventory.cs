using System;
using System.Collections.Generic;
using Stats;
using Units;

namespace Gears
{
    [Serializable]
    public class Inventory
    {
        public List<Gear> gears = new List<Gear>();

        public Inventory() {}

        public Inventory(Unit _unit)
        {
            gears = new List<Gear>();
            foreach (Gear _gear in _unit.Inventory.gears)
            {
                gears.Add(new Gear(_gear));
            }
        }
        public Inventory(SaveInventory _save)
        {
            gears = new List<Gear>(_save.gears);
        }

        public void GenerateLootGear()
        {
            Gear gear = new Gear();
            gear.CreateGear();
            gears.Add(gear);
        }
        
        public void GenerateGearFor(MonsterSO monster)
        {
            //TODO : create archetype of monster like Mage, Warrior, Rogue and add a List of Gear tha can't be used for each archetype then in CreateGear take random but the List (in MonsterSO do a EArchetype)
            Gear gear = new Gear();
            gear.CreateGear();
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