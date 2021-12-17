using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using Gears;
using GridObjects;
using StateMachine;
using TMPro;
using Units;
using UnityEngine;

namespace UserInterface.BattleScene
{
    public class BattleInventory_UI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI HeroName;
        [SerializeField] private TextMeshProUGUI MonsterName;
        [SerializeField] private GameObject prefabGear;
        [SerializeField] private List<DragAndDropCell> HeroSlots;
        [SerializeField] private List<DragAndDropCell> MonsterSlots;
        private Inventory heroInventory;
        private Inventory monsterInventory;

        public List<Unit> Queue = new List<Unit>();

        [Header("Event Sender")]
        [SerializeField] private VoidEvent onUIEnable;
        [SerializeField] private VoidEvent onActionDone;

        public void OpenBox(GridObject lootBox)
        {
            onUIEnable.Raise();
            
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
               if (!Queue.Contains(_monster))
                    Queue.Add(_monster);

               if (Queue.Count == 1)
                    onUIEnable.Raise();
                
               HeroSlots.ForEach(cell => cell.RemoveItem());
               MonsterSlots.ForEach(cell => cell.RemoveItem());
                
               HeroName.text = BattleStateManager.instance.PlayingUnit.UnitName;
               MonsterName.text = Queue[0].UnitName;
                
               monsterInventory = Queue[0].Inventory;
               heroInventory = BattleStateManager.instance.PlayingUnit.Inventory;
               showGear();
        }

        private void showGear()
        {
            onUIEnable.Raise();
            
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
        
        public void CloseInventory(GameObject closeBtn)
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

            if (Queue.Count > 0)
                Queue.Remove(Queue[0]);
            
            if (Queue.Count > 0)
            {
                ShowOnKill(Queue[0]);
                return;
            }

            gameObject.SetActive(false);
            closeBtn.SetActive(false);
            onActionDone.Raise();
        }
    }
}