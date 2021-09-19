using System;
using System.Collections.Generic;
using System.Linq;
using _DragAndDropSystem;
using Grid;
using GridObjects;
using Stats;
using TMPro;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gears
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI HeroName;
        [SerializeField] private TextMeshProUGUI MonsterName;
        [SerializeField] private GameObject prefabGear;
        [SerializeField] private List<DragAndDropCell> HeroSlots;
        [SerializeField] private List<DragAndDropCell> MonsterSlots;
        private Inventory heroInventory;
        private Inventory monsterInventory;

        public void OpenBox(GridObject lootBox)
        {
            BattleStateManager.instance.BlockInputs();
            
            HeroSlots.ForEach(cell => cell.RemoveItem());
            MonsterSlots.ForEach(cell => cell.RemoveItem());

            HeroName.text = BattleStateManager.instance.PlayingUnit.UnitName;
            MonsterName.text = lootBox.GridObjectSO.Name;

            monsterInventory = lootBox.Inventory;
            heroInventory = BattleStateManager.instance.PlayingUnit.Inventory;

            showGear();
        }
        
        public void ShowOnKill(Unit _monster)
        {
            BattleStateManager.instance.BlockInputs();
            
            HeroSlots.ForEach(cell => cell.RemoveItem());
            MonsterSlots.ForEach(cell => cell.RemoveItem());
            
            HeroName.text = BattleStateManager.instance.PlayingUnit.UnitName;
            MonsterName.text = _monster.UnitName;
            
            monsterInventory = _monster.Inventory;
            heroInventory = BattleStateManager.instance.PlayingUnit.Inventory;

            showGear();
        }

        private void showGear()
        {
            BattleStateManager.instance.BlockInputs();
            
            for (int i = 0; i < heroInventory.gears.Count; i++)
            {
                GameObject pref = Instantiate(prefabGear, HeroSlots[i].transform);
                pref.GetComponent<InfoGear>().Gear = heroInventory.gears[i];
                pref.GetComponent<InfoGear>().DisplayIcon();
            }
            
            for (int i = 0; i < monsterInventory.gears.Count; i++)
            {
                GameObject pref = Instantiate(prefabGear, MonsterSlots[i].transform);
                pref.GetComponent<InfoGear>().Gear = monsterInventory.gears[i];
                pref.GetComponent<InfoGear>().DisplayIcon();
            }
        }
        
        public void CloseInventory()
        {
            if (monsterInventory != null)
            {
                monsterInventory.gears = new List<Gear>();
                
                foreach (DragAndDropCell _dropCell in MonsterSlots)
                {
                    if(_dropCell.GetInfoGear() != null)
                        monsterInventory.gears.Add(_dropCell.GetInfoGear().Gear);
                }
            }
                
            heroInventory.gears = new List<Gear>();
            
            foreach (DragAndDropCell _dropCell in HeroSlots)
            {
                if(_dropCell.GetInfoGear() != null)
                    heroInventory.gears.Add(_dropCell.GetInfoGear().Gear);
            }
            
            BattleStateManager.instance.PlayingUnit.UpdateStats();
            
            gameObject.SetActive(false);
            
            BattleStateManager.instance.OnSkillUsed();
        }
    }
}