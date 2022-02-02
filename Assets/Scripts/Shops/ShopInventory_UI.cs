using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Instances;
using Gears;
using Units;
using UnityEngine;
using UserInterface;
using Void = _EventSystem.CustomEvents.Void;

namespace Shops
{
    public class ShopInventory_UI : MonoBehaviour
    {
        [SerializeField] private List<PersonalInventory> Portraits;
        [SerializeField] private GameObject prefabGear;
        [SerializeField] private List<BattleHero> battleHeroes;

        [SerializeField] private VoidEvent onItemMoved;

        private void Start()
        {
            for (int i = 0; i < Portraits.Count; i++)
            {
                PlayerData.getInstance().Heroes[i].Spawn(battleHeroes[i]);
                Portraits[i].Initialize(PlayerData.getInstance().Heroes[i]);

                for (int j = 0; j < Portraits[i].Hero.Inventory.gears.Count; j++)
                {
                    GameObject pref = Instantiate(prefabGear, Portraits[i].Slots[j].transform);
                    pref.GetComponent<GearInfo>().Gear = Portraits[i].Hero.Inventory.gears[j];
                    pref.GetComponent<GearInfo>().DisplayIcon();
                }
            }
            onItemMoved.EventListeners += UpdateInventories;
        }

        private void OnDestroy()
        {
            onItemMoved.EventListeners -= UpdateInventories;
        }

        private void UpdateInventories(Void empty)
        {
            for (int i = 0; i < Portraits.Count; i++)
            {
                Portraits[i].Hero.Inventory.gears = new List<Gear>();
            
                foreach (DragAndDropCell _dropCell in Portraits[i].Slots)
                {
                    if(_dropCell.GetInfoGear() != null)
                        Portraits[i].Hero.Inventory.gears.Add(_dropCell.GetInfoGear().Gear);
                }
                
                Portraits[i].UpdateStats();
            }
        }
    }
}