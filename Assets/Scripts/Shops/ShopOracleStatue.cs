using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Instances;
using Relics;
using Units;
using UnityEngine;
using UserInterface.BattleScene;

namespace Shops
{
    public class ShopOracleStatue : MonoBehaviour
    {
        [SerializeField] private SlotDragAndDrop RelicSlot;
        
        [SerializeField] private GameObject prefabRelic;
        [SerializeField] private List<HeroRelicSlot> heroRelicSlots;


        public void Start()
        {
            for (int i = 0; i < PlayerData.getInstance().Heroes.Count; i++)
            {
                heroRelicSlots[i].Initialize(PlayerData.getInstance().Heroes[i]);
            }

            showRelics();

        }

        private void showRelics()
        {
            GameObject pref = Instantiate(prefabRelic, RelicSlot.transform);
            pref.GetComponent<RelicInfo>().CreateRelic(DataBase.Relic.GetRandom());
            pref.GetComponent<RelicInfo>().DisplayIcon();
            RelicSlot.UpdateMyItem();
            RelicSlot.UpdateBackgroundState();
        }
        
        public void AscendBtn()
        {
            foreach (HeroRelicSlot _heroRelicSlot in heroRelicSlots)
            {
                _heroRelicSlot.ApplyAndClose();
            }
            for (int i = 0; i < PlayerData.getInstance().Heroes.Count; i++)
            {
                heroRelicSlots[i].Initialize(PlayerData.getInstance().Heroes[i]);
            }
        }
    }
}