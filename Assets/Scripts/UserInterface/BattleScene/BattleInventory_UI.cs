using System;
using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using Gears;
using GridObjects;
using StateMachine;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.BattleScene
{
    public class BattleInventory_UI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI HeroName;
        [SerializeField] private Image heroSprite;
        [SerializeField] private TextMeshProUGUI MonsterName;
        [SerializeField] private Image monsterSprite;
        [SerializeField] private GameObject prefabGear;
        [SerializeField] private List<SlotDragAndDrop> HeroSlots;
        [SerializeField] private List<SlotDragAndDrop> MonsterSlots;
        [SerializeField] private GameObject Blur;
        private Inventory heroInventory;
        private Inventory monsterInventory;

        private Unit monster;

        [Header("Event Sender")]
        [SerializeField] private VoidEvent onUIEnable;
        [SerializeField] private VoidEvent onActionDone;
        [SerializeField] private UnitEvent onInventoryClosed;

        private void OnEnable()
        {
            Blur.SetActive(true);
        }

        private void OnDisable()
        {
            Blur.SetActive(false);
        }

        public void OpenBox(GridObject lootBox)
        {
            onUIEnable.Raise();
            
            HeroSlots.ForEach(cell => cell.RemoveItem());
            MonsterSlots.ForEach(cell => cell.RemoveItem());

            HeroName.text = BattleStateManager.instance.PlayingUnit.UnitName;
            heroSprite.sprite = BattleStateManager.instance.PlayingUnit.GetIcon();
            MonsterName.text = lootBox.GridObjectSO.Name;
            monsterSprite.sprite = lootBox.GridObjectSO.GetIcon();

            monsterInventory = lootBox.Inventory;
            heroInventory = BattleStateManager.instance.PlayingUnit.Inventory;

            showGear();
        }
        
        public void ShowOnKill(Unit _monster)
        {
            onUIEnable.Raise();

            monster = _monster;
            
            HeroSlots.ForEach(cell => cell.RemoveItem());
            MonsterSlots.ForEach(cell => cell.RemoveItem());
            
            HeroName.text = BattleStateManager.instance.PlayingUnit.UnitName;
            heroSprite.sprite = BattleStateManager.instance.PlayingUnit.GetIcon();
            MonsterName.text = monster.UnitName;
            monsterSprite.sprite = monster.GetIcon();
            
            monsterInventory = _monster.Inventory;
            heroInventory = BattleStateManager.instance.PlayingUnit.Inventory; 
            showGear();
        }

        private void showGear()
        {
            onUIEnable.Raise();
            
            for (int i = 0; i < heroInventory.gears.Count; i++)
            {
                GameObject pref = Instantiate(prefabGear, HeroSlots[i].transform);
                pref.GetComponent<GearInfo>().Gear = heroInventory.gears[i];
                pref.GetComponent<GearInfo>().DisplayIcon();
                HeroSlots[i].UpdateMyItem();
                HeroSlots[i].UpdateBackgroundState();
            }
            
            for (int i = 0; i < monsterInventory.gears.Count; i++)
            {
                GameObject pref = Instantiate(prefabGear, MonsterSlots[i].transform);
                pref.GetComponent<GearInfo>().Gear = monsterInventory.gears[i];
                pref.GetComponent<GearInfo>().DisplayIcon();
                MonsterSlots[i].UpdateMyItem();
                MonsterSlots[i].UpdateBackgroundState();
            }
        }
        
        public void CloseInventory(GameObject closeBtn)
        {
            if (monsterInventory != null)
            {
                monsterInventory.gears = new List<Gear>();
                
                foreach (SlotDragAndDrop _dropCell in MonsterSlots)
                {
                    if(_dropCell.GetInfoGear() != null)
                        monsterInventory.gears.Add(_dropCell.GetInfoGear().Gear);
                }
            }
                
            heroInventory.gears = new List<Gear>();
            
            foreach (SlotDragAndDrop _dropCell in HeroSlots)
            {
                if(_dropCell.GetInfoGear() != null)
                    heroInventory.gears.Add(_dropCell.GetInfoGear().Gear);
            }
            
            BattleStateManager.instance.PlayingUnit.UpdateStats();

            gameObject.SetActive(false);
            closeBtn.SetActive(false);
            onActionDone.Raise();

            onInventoryClosed.Raise(monster);
        }
    }
}