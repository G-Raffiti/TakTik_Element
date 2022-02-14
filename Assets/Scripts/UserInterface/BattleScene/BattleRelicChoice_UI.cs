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
using UnityEngine.UI;

namespace UserInterface.BattleScene
{
    public class BattleRelicChoice_UI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI MonsterName;
        [SerializeField] private Image monsterSprite;
        [SerializeField] private GameObject prefabRelic;
        [SerializeField] private List<SlotDragAndDrop> MonsterSlots;
        [SerializeField] private List<HeroRelicSlot> heroRelicSlots;
        private List<RelicSO> monsterRelics;
        [SerializeField] private GameObject Blur;

        [Header("Event Sender")]
        [SerializeField] private VoidEvent onUIEnable;
        [SerializeField] private VoidEvent onActionDone;

        
        private void OnEnable()
        {
            onUIEnable.Raise();
            Blur.SetActive(true);
        }

        private void OnDisable()
        {
            Blur.SetActive(false);
        }

        public void ShowOnKill(Unit _unit)
        {
            Monster _monster = (Monster) _unit;
            for (int i = 0; i < PlayerData.getInstance().Heroes.Count; i++)
            {
                heroRelicSlots[i].Initialize(PlayerData.getInstance().Heroes[i]);
            }
            
            MonsterSlots.ForEach(cell => cell.RemoveItem());
            MonsterName.text = _monster.UnitName;
            monsterSprite.sprite = _monster.UnitSprite;

            monsterRelics = new List<RelicSO>();
            monsterRelics.AddRange(_monster.Relics);

            showRelics();

        }

        private void showRelics()
        {
            for (int i = 0; i < monsterRelics.Count; i++)
            {
                GameObject pref = Instantiate(prefabRelic, MonsterSlots[i].transform);
                pref.GetComponent<RelicInfo>().CreateRelic(monsterRelics[i]);
                pref.GetComponent<RelicInfo>().DisplayIcon();
                MonsterSlots[i].UpdateMyItem();
                MonsterSlots[i].UpdateBackgroundState();
            }

            foreach (SlotDragAndDrop _slot in MonsterSlots.Where(m => m.GetInfoRelic() == null))
            {
                GameObject pref = Instantiate(prefabRelic, _slot.transform);
                pref.GetComponent<RelicInfo>().CreateRelic(DataBase.Relic.GetRandom());
                pref.GetComponent<RelicInfo>().DisplayIcon();
                _slot.UpdateMyItem();
                _slot.UpdateBackgroundState();
            }
        }
        
        public void ApplyAndClose()
        {
            if (monsterRelics != null)
            {
                monsterRelics = new List<RelicSO>();
                
                foreach (SlotDragAndDrop _slot in MonsterSlots)
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