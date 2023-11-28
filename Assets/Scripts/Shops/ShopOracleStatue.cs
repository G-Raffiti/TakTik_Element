using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Instances;
using Relics;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using UserInterface.BattleScene;

namespace Shops
{
    public class ShopOracleStatue : MonoBehaviour
    {
        [FormerlySerializedAs("RelicSlot")]
        [SerializeField] private SlotDragAndDrop relicSlot;
        
        [SerializeField] private GameObject prefabRelic;
        [SerializeField] private List<HeroRelicSlot> heroRelicSlots;


        public void Start()
        {
            for (int _i = 0; _i < PlayerData.GetInstance().Heroes.Count; _i++)
            {
                heroRelicSlots[_i].Initialize(PlayerData.GetInstance().Heroes[_i]);
            }

            ShowRelics();
        }

        private void ShowRelics()
        {
            GameObject _pref = Instantiate(prefabRelic, relicSlot.transform);
            _pref.GetComponent<RelicInfo>().CreateRelic(DataBase.Relic.GetRandom());
            _pref.GetComponent<RelicInfo>().DisplayIcon();
            relicSlot.UpdateMyItem();
            relicSlot.UpdateBackgroundState();
        }
        
        public void AscendBtn()
        {
            foreach (HeroRelicSlot _heroRelicSlot in heroRelicSlots)
            {
                _heroRelicSlot.ApplyAndClose();
            }
            for (int _i = 0; _i < PlayerData.GetInstance().Heroes.Count; _i++)
            {
                heroRelicSlots[_i].Initialize(PlayerData.GetInstance().Heroes[_i]);
            }
        }
    }
}