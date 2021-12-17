﻿using System.Collections.Generic;
using System.Linq;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Instances;
using Relics;
using Skills;
using TMPro;
using Units;
using UnityEngine;

namespace UserInterface.BattleScene
{
    public class BattleRelicChoice_UI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI MonsterName;
        [SerializeField] private GameObject prefabRelic;
        [SerializeField] private List<DragAndDropCell> HeroSlots;
        [SerializeField] private List<DragAndDropCell> MonsterSlots;
        private List<RelicSO> monsterRelics;
        [SerializeField] private Hero hero1;
        [SerializeField] private Hero hero2;
        [SerializeField] private Hero hero3;

        [Header("Event Sender")]
        [SerializeField] private VoidEvent onUIEnable;
        [SerializeField] private VoidEvent onActionDone;

        public void ShowOnKill(Unit _unit)
        {
            Monster _monster = (Monster) _unit;
            
            onUIEnable.Raise();
            
            HeroSlots.ForEach(cell => cell.RemoveItem());
            MonsterSlots.ForEach(cell => cell.RemoveItem());
            MonsterName.text = _monster.UnitName;

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
            }

            foreach (DragAndDropCell cell in MonsterSlots.Where(m => m.GetInfoRelic() == null))
            {
                GameObject pref = Instantiate(prefabRelic, cell.transform);
                pref.GetComponent<RelicInfo>().CreateRelic(DataBase.Relic.GetRandom());
                pref.GetComponent<RelicInfo>().DisplayIcon();
            }
        }
        
        public void ApplyAndClose()
        {
            if (monsterRelics != null)
            {
                monsterRelics = new List<RelicSO>();
                
                foreach (DragAndDropCell _dropCell in MonsterSlots)
                {
                    if(_dropCell.GetInfoRelic() != null)
                        monsterRelics.Add(_dropCell.GetInfoRelic().Relic);
                }
            }

            for (int i = 0; i < HeroSlots.Count; i++)
            {
                if (i == 0 && HeroSlots[0].GetInfoRelic() != null)
                {
                    hero1.AddRelic(HeroSlots[0].GetInfoRelic().Relic);
                }

                if (i == 1 && HeroSlots[1].GetInfoRelic() != null)
                {
                    hero2.AddRelic(HeroSlots[1].GetInfoRelic().Relic);
                }
                
                if (i == 2 && HeroSlots[2].GetInfoRelic() != null)
                {
                    hero3.AddRelic(HeroSlots[2].GetInfoRelic().Relic);
                }
            }

            gameObject.SetActive(false);
            
            onActionDone.Raise();
        }
    }
}