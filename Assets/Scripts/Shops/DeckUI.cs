using System;
using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Instances;
using Skills;
using Units;
using UnityEngine;
using UserInterface;
using Void = _EventSystem.CustomEvents.Void;

namespace Shops
{
    public class DeckUI : MonoBehaviour
    {
        [SerializeField] private DragAndDropCell prefabSlot;
        
        [SerializeField] private DragAndDropItem prefabSkill;
        [SerializeField] private DragAndDropItem prefabRelic;
        
        [SerializeField] private List<Transform> deckPlaceHolders;
        [SerializeField] private List<Transform> relicPlaceHolders;
        [SerializeField] private List<PersonalInventory> portraits;
        [SerializeField] private BattleHero actualHero;
        private List<SkillInfo> allSkills { get; set; }

        [Header("Event Listener")]
        [SerializeField] private IntEvent onCampPointUsed;

        private void OnEnable()
        {
            onCampPointUsed.EventListeners += InitializeDisplay;
            
            for (int i = 0; i < PlayerData.getInstance().Heroes.Count; i++)
            {
                portraits[i].Initialize(PlayerData.getInstance().Heroes[i]);
            }
            
            PlayerData.getInstance().Heroes[0].Spawn(actualHero.gameObject);
            
            InitializeDisplay(0);
        }

        private void OnDisable()
        {
            onCampPointUsed.EventListeners -= InitializeDisplay;
        }


        public void InitializeDisplay(int empty)
        {
            ClearDecks();

            List<Deck> _allDecks = GameObject.Find("Decks").GetComponent<AllDecks>().Decks;
            for (int j = 0; j < _allDecks.Count; j++)
            {
                for (int i = 0; i < _allDecks[j].Skills.Count; i++)
                {
                    GameObject _cell = Instantiate(prefabSlot.gameObject, deckPlaceHolders[j]);
                    _cell.GetComponent<DragAndDropCell>().cellType = DragAndDropCell.CellType.DragOnly;
                    _cell.GetComponent<DragAndDropCell>().unlimitedSource = true;
                    _cell.GetComponent<DragAndDropCell>().containType = DragAndDropCell.ContainType.Skill;
                    
                    GameObject pref = Instantiate(prefabSkill.gameObject, _cell.transform);
                    pref.GetComponent<SkillInfo>().Deck = _allDecks[j];
                    pref.GetComponent<SkillInfo>().UpdateSkill(_allDecks[j].Skills[i], actualHero);
                    pref.GetComponent<SkillInfo>().DisplayIcon();
                    
                    allSkills.Add(pref.GetComponent<SkillInfo>());
                }
            }
            
            for (int j = 0; j < _allDecks.Count; j++)
            {
                for (int i = 0; i < _allDecks[j].Relics.Count; i++)
                {
                    GameObject _cell = Instantiate(prefabSlot.gameObject, relicPlaceHolders[j]);
                    _cell.GetComponent<DragAndDropCell>().cellType = DragAndDropCell.CellType.DragOnly;
                    _cell.GetComponent<DragAndDropCell>().unlimitedSource = true;
                    _cell.GetComponent<DragAndDropCell>().containType = DragAndDropCell.ContainType.Relic;
                    
                    GameObject pref = Instantiate(prefabRelic.gameObject, _cell.transform);
                    pref.GetComponent<RelicInfo>().CreateRelic(_allDecks[j].Relics[i]);
                    pref.GetComponent<RelicInfo>().SetDeck(_allDecks[j]);
                    pref.GetComponent<RelicInfo>().DisplayIcon();
                }
            }
        }

        private void ClearDecks()
        {
            foreach (Transform _deckPlaceHolder in deckPlaceHolders)
            {
                while (_deckPlaceHolder.childCount > 0)
                {
                    GameObject.DestroyImmediate(_deckPlaceHolder.GetChild(0).gameObject);
                }
            }

            foreach (Transform _relicPlaceHolder in relicPlaceHolders)
            {
                while (_relicPlaceHolder.childCount > 0)
                {
                    DestroyImmediate(_relicPlaceHolder.GetChild(0).gameObject);
                }
            }
            
            allSkills = new List<SkillInfo>();
        }

        public void ChangeHero(int _index)
        {
            PlayerData.getInstance().Heroes[_index].Spawn(actualHero.gameObject);
            foreach (SkillInfo _skill in allSkills)
            {
                _skill.UpdateSkill(_skill.Skill, actualHero);
            }
        }
    }
}