using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using Gears;
using Grid;
using GridObjects;
using TMPro;
using Units;
using UnityEngine;

namespace Skills
{
    public class RelicChoiceUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI MonsterName;
        [SerializeField] private GameObject prefabRelic;
        [SerializeField] private List<DragAndDropCell> DecksSlots;
        [SerializeField] private List<DragAndDropCell> MonsterSlots;
        private List<RelicSO> monsterRelics;
        [SerializeField] private DeckMono deck1;
        //[SerializeField] private DeckMono deck2;
        //[SerializeField] private DeckMono deck3;

        [Header("Event Sender")]
        [SerializeField] private VoidEvent onUIEnable;
        [SerializeField] private VoidEvent onActionDone;

        public void ShowOnKill(Unit _unit)
        {
            Monster _monster = (Monster) _unit;
            
            onUIEnable.Raise();
            
            DecksSlots.ForEach(cell => cell.RemoveItem());
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

            for (int i = 0; i < DecksSlots.Count; i++)
            {
                if (i == 0 && DecksSlots[0].GetInfoRelic() != null)
                {
                    deck1.Relics.Add(DecksSlots[0].GetInfoRelic().Relic);
                    deck1.UpdateDeck();
                }

                /*if (i == 1 && DecksSlots[1].GetInfoRelic() != null)
                {
                    deck2.Relics.Add(DecksSlots[1].GetInfoRelic().Relic);
                    deck2.UpdateDeck();
                }
                
                if (i == 2 && DecksSlots[2].GetInfoRelic() != null)
                {
                    deck3.Relics.Add(DecksSlots[2].GetInfoRelic().Relic);
                    deck3.UpdateDeck();
                }*/
            }

            gameObject.SetActive(false);
            
            onActionDone.Raise();
        }
    }
}