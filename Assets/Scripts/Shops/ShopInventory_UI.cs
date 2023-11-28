using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Instances;
using Gears;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using UserInterface;
using Void = _EventSystem.CustomEvents.Void;

namespace Shops
{
    public class ShopInventoryUI : MonoBehaviour
    {
        [FormerlySerializedAs("Portraits")]
        [SerializeField] private List<PersonalInventory> portraits;
        [SerializeField] private GameObject prefabGear;
        [SerializeField] private List<BattleHero> battleHeroes;

        [SerializeField] private VoidEvent onItemMoved;

        private void Start()
        {
            for (int _i = 0; _i < portraits.Count; _i++)
            {
                PlayerData.GetInstance().Heroes[_i].Spawn(battleHeroes[_i]);
                portraits[_i].Initialize(PlayerData.GetInstance().Heroes[_i]);
                portraits[_i].FillInventory();
            }
            onItemMoved.EventListeners += UpdateInventories;
        }

        private void OnDestroy()
        {
            onItemMoved.EventListeners -= UpdateInventories;
        }

        private void UpdateInventories(Void _empty)
        {
            for (int _i = 0; _i < portraits.Count; _i++)
            {
                portraits[_i].Hero.Inventory.gears = new List<Gear>();
            
                foreach (SlotDragAndDrop _dropCell in portraits[_i].Slots)
                {
                    if(_dropCell.GetInfoGear() != null)
                        portraits[_i].Hero.Inventory.gears.Add(_dropCell.GetInfoGear().Gear);
                }
                
                portraits[_i].UpdateStats();
            }
        }
    }
}