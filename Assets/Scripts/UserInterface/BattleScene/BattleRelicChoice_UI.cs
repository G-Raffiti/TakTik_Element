using System;
using System.Collections.Generic;
using System.Linq;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Instances;
using Relics;
using Skills;
using TMPro;
using TMPro.SpriteAssetUtilities;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UserInterface.BattleScene
{
    public class BattleRelicChoiceUI : MonoBehaviour
    {
        [FormerlySerializedAs("MonsterName")]
        [SerializeField] private TextMeshProUGUI monsterName;
        [SerializeField] private Image monsterSprite;
        [SerializeField] private GameObject prefabRelic;
        [FormerlySerializedAs("MonsterSlots")]
        [SerializeField] private List<SlotDragAndDrop> monsterSlots;
        [SerializeField] private List<HeroRelicSlot> heroRelicSlots;
        private List<RelicSo> monsterRelics;
        [FormerlySerializedAs("Blur")]
        [SerializeField] private GameObject blur;

        [Header("Event Sender")]
        [SerializeField] private VoidEvent onUIEnable;
        [SerializeField] private VoidEvent onActionDone;

        
        private void OnEnable()
        {
            onUIEnable.Raise();
            blur.SetActive(true);
        }

        private void OnDisable()
        {
            blur.SetActive(false);
        }

        public void ShowOnKill(Unit _unit)
        {
            Monster _monster = (Monster) _unit;
            for (int _i = 0; _i < PlayerData.GetInstance().Heroes.Count; _i++)
            {
                heroRelicSlots[_i].Initialize(PlayerData.GetInstance().Heroes[_i]);
            }
            
            monsterSlots.ForEach(_cell => _cell.RemoveItem());
            monsterName.text = _monster.unitName;
            monsterSprite.sprite = _monster.UnitSprite;

            monsterRelics = new List<RelicSo>();
            monsterRelics.AddRange(_monster.Relics);

            ShowRelics();

        }

        private void ShowRelics()
        {
            for (int _i = 0; _i < monsterRelics.Count; _i++)
            {
                GameObject _pref = Instantiate(prefabRelic, monsterSlots[_i].transform);
                _pref.GetComponent<RelicInfo>().CreateRelic(monsterRelics[_i]);
                _pref.GetComponent<RelicInfo>().DisplayIcon();
                monsterSlots[_i].UpdateMyItem();
                monsterSlots[_i].UpdateBackgroundState();
            }

            foreach (SlotDragAndDrop _slot in monsterSlots.Where(_m => _m.GetInfoRelic() == null))
            {
                GameObject _pref = Instantiate(prefabRelic, _slot.transform);
                _pref.GetComponent<RelicInfo>().CreateRelic(DataBase.Relic.GetRandom());
                _pref.GetComponent<RelicInfo>().DisplayIcon();
                _slot.UpdateMyItem();
                _slot.UpdateBackgroundState();
            }
        }
        
        public void ApplyAndClose()
        {
            if (monsterRelics != null)
            {
                monsterRelics = new List<RelicSo>();
                
                foreach (SlotDragAndDrop _slot in monsterSlots)
                {
                    if(_slot.GetInfoRelic() != null)
                        monsterRelics.Add(_slot.GetInfoRelic().Relic);
                }
            }

            foreach (HeroRelicSlot _heroRelicSlot in heroRelicSlots)
            {
                _heroRelicSlot.ApplyAndClose();
            }
            
            onActionDone.Raise();
            gameObject.SetActive(false);
        }
    }
}