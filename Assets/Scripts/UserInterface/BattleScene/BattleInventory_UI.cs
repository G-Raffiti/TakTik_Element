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
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UserInterface.BattleScene
{
    public class BattleInventoryUI : MonoBehaviour
    {
        [FormerlySerializedAs("HeroName")]
        [SerializeField] private TextMeshProUGUI heroName;
        [SerializeField] private Image heroSprite;
        [FormerlySerializedAs("MonsterName")]
        [SerializeField] private TextMeshProUGUI monsterName;
        [SerializeField] private Image monsterSprite;
        [SerializeField] private GameObject prefabGear;
        [FormerlySerializedAs("HeroSlots")]
        [SerializeField] private List<SlotDragAndDrop> heroSlots;
        [FormerlySerializedAs("MonsterSlots")]
        [SerializeField] private List<SlotDragAndDrop> monsterSlots;
        [FormerlySerializedAs("Blur")]
        [SerializeField] private GameObject blur;
        private Inventory heroInventory;
        private Inventory monsterInventory;

        private Unit monster;

        [Header("Event Sender")]
        [SerializeField] private VoidEvent onUIEnable;
        [SerializeField] private VoidEvent onActionDone;
        [SerializeField] private UnitEvent onInventoryClosed;

        private void OnEnable()
        {
            blur.SetActive(true);
        }

        private void OnDisable()
        {
            blur.SetActive(false);
        }

        public void OpenBox(GridObject _lootBox)
        {
            onUIEnable.Raise();
            
            heroSlots.ForEach(_cell => _cell.RemoveItem());
            monsterSlots.ForEach(_cell => _cell.RemoveItem());

            heroName.text = BattleStateManager.instance.PlayingUnit.unitName;
            heroSprite.sprite = BattleStateManager.instance.PlayingUnit.GetIcon();
            monsterName.text = _lootBox.GridObjectSo.Name;
            monsterSprite.sprite = _lootBox.GridObjectSo.GetIcon();

            monsterInventory = _lootBox.inventory;
            heroInventory = BattleStateManager.instance.PlayingUnit.inventory;

            ShowGear();
        }
        
        public void ShowOnKill(Unit _monster)
        {
            onUIEnable.Raise();

            monster = _monster;
            
            heroSlots.ForEach(_cell => _cell.RemoveItem());
            monsterSlots.ForEach(_cell => _cell.RemoveItem());
            
            heroName.text = BattleStateManager.instance.PlayingUnit.unitName;
            heroSprite.sprite = BattleStateManager.instance.PlayingUnit.GetIcon();
            monsterName.text = monster.unitName;
            monsterSprite.sprite = monster.GetIcon();
            
            monsterInventory = _monster.inventory;
            heroInventory = BattleStateManager.instance.PlayingUnit.inventory; 
            ShowGear();
        }

        private void ShowGear()
        {
            onUIEnable.Raise();
            
            for (int _i = 0; _i < heroInventory.gears.Count; _i++)
            {
                GameObject _pref = Instantiate(prefabGear, heroSlots[_i].transform);
                _pref.GetComponent<GearInfo>().Gear = heroInventory.gears[_i];
                _pref.GetComponent<GearInfo>().DisplayIcon();
                heroSlots[_i].UpdateMyItem();
                heroSlots[_i].UpdateBackgroundState();
            }
            
            for (int _i = 0; _i < monsterInventory.gears.Count; _i++)
            {
                GameObject _pref = Instantiate(prefabGear, monsterSlots[_i].transform);
                _pref.GetComponent<GearInfo>().Gear = monsterInventory.gears[_i];
                _pref.GetComponent<GearInfo>().DisplayIcon();
                monsterSlots[_i].UpdateMyItem();
                monsterSlots[_i].UpdateBackgroundState();
            }
        }
        
        public void CloseInventory(GameObject _closeBtn)
        {
            if (monsterInventory != null)
            {
                monsterInventory.gears = new List<Gear>();
                
                foreach (SlotDragAndDrop _dropCell in monsterSlots)
                {
                    if(_dropCell.GetInfoGear() != null)
                        monsterInventory.gears.Add(_dropCell.GetInfoGear().Gear);
                }
            }
                
            heroInventory.gears = new List<Gear>();
            
            foreach (SlotDragAndDrop _dropCell in heroSlots)
            {
                if(_dropCell.GetInfoGear() != null)
                    heroInventory.gears.Add(_dropCell.GetInfoGear().Gear);
            }
            
            BattleStateManager.instance.PlayingUnit.UpdateStats();

            gameObject.SetActive(false);
            _closeBtn.SetActive(false);
            onActionDone.Raise();
            
            if (BattleStateManager.instance.DeadThisTurn.Count > 0)
                BattleStateManager.instance.DeadThisTurn.Remove(BattleStateManager.instance.DeadThisTurn[0]);
        }
    }
}