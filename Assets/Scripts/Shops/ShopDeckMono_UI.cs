using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Instances;
using Relics;
using Skills;
using Units;
using UnityEngine;
using UserInterface;

namespace Shops
{
    public class ShopDeckMono_UI : MonoBehaviour
    {
        [SerializeField] private DragAndDropCell prefabSlot;

        [SerializeField] private DragAndDropItem prefabSkill;
        [SerializeField] private DragAndDropItem prefabRelic;

        [SerializeField] private Transform deckPlaceHolder;
        [SerializeField] private Transform relicPlaceHolder;
        [SerializeField] private List<PersonalInventory> portraits;
        [SerializeField] private BattleHero actualHero;
        private List<SkillInfo> allSkills { get; set; }

        [Header("Event Listener")] 
        [SerializeField] private IntEvent onCampPointUsed;
        [SerializeField] private IntEvent onHeroChange;

        private void OnEnable()
        {
            onHeroChange.EventListeners += ChangeHero;
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
            onHeroChange.EventListeners -= ChangeHero;
            onCampPointUsed.EventListeners -= InitializeDisplay;
        }


        public void InitializeDisplay(int empty)
        {
            ClearDecks();

            DeckMono Deck = GameObject.FindObjectOfType<DeckMono>();
            Deck.UpdateDeck();
            
            foreach (SkillSO _skillSO in Deck.Skills)
            {
                GameObject _cell = GameObject.Instantiate(prefabSlot.gameObject, deckPlaceHolder);
                _cell.GetComponent<DragAndDropCell>().cellType = DragAndDropCell.CellType.DragOnly;
                _cell.GetComponent<DragAndDropCell>().unlimitedSource = true;
                _cell.GetComponent<DragAndDropCell>().containType = DragAndDropCell.ContainType.Skill;

                GameObject pref = GameObject.Instantiate(prefabSkill.gameObject, _cell.transform);
                pref.GetComponent<SkillInfo>().skill = Skill.CreateSkill(_skillSO, Deck, actualHero);
                pref.GetComponent<SkillInfo>().Unit = actualHero;
                pref.GetComponent<SkillInfo>().DisplayIcon();

                allSkills.Add(pref.GetComponent<SkillInfo>());
            }

            foreach (RelicSO _relicSO in Deck.Relics)
            {
                GameObject _cell = GameObject.Instantiate(prefabSlot.gameObject, relicPlaceHolder);
                _cell.GetComponent<DragAndDropCell>().cellType = DragAndDropCell.CellType.DragOnly;
                _cell.GetComponent<DragAndDropCell>().unlimitedSource = true;
                _cell.GetComponent<DragAndDropCell>().containType = DragAndDropCell.ContainType.Relic;

                GameObject pref = GameObject.Instantiate(prefabRelic.gameObject, _cell.transform);
                pref.GetComponent<RelicInfo>().CreateRelic(_relicSO);
                pref.GetComponent<RelicInfo>().DisplayIcon();
            }
        }

        private void ClearDecks()
        {
            
            while (deckPlaceHolder.childCount > 0)
            {
                GameObject.DestroyImmediate(deckPlaceHolder.GetChild(0).gameObject);
            }

            while (relicPlaceHolder.childCount > 0)
            {
                GameObject.DestroyImmediate(relicPlaceHolder.GetChild(0).gameObject);
            }
            

            allSkills = new List<SkillInfo>();
        }

        public void ChangeHero(int _index)
        {
            DeckMono Deck = GameObject.FindObjectOfType<DeckMono>();
            PlayerData.getInstance().Heroes[_index].Spawn(actualHero.gameObject);
            foreach (SkillInfo _skill in allSkills)
            {
                SkillSO s = _skill.skill.BaseSkill;
                _skill.skill = Skill.CreateSkill(s, Deck, actualHero);
            }
        }
    }
}