using System;
using System.Collections.Generic;
using System.Linq;
using _Instances;
using Gears;
using Players;
using Resources.ToolTip.Scripts;
using Stats;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class Inventory_UI : MonoBehaviour
    {
        [SerializeField] private List<PersonalInventory> Portraits;
        [SerializeField] private GameObject prefabGear;

        private void Start()
        {
            for (int i = 0; i < Portraits.Count; i++)
            {
                Portraits[i].Initialize(PlayerData.getInstance().Heroes[i]);

                for (int j = 0; j < Portraits[i].Hero.Inventory.gears.Count; j++)
                {
                    GameObject pref = Instantiate(prefabGear, Portraits[i].Slots[j].transform);
                    pref.GetComponent<InfoGear>().Gear = Portraits[i].Hero.Inventory.gears[j];
                    pref.GetComponent<InfoGear>().DisplayIcon();
                }
            }
        }
    }
}