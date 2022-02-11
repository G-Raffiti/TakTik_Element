using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Instances;
using Decks;
using Relics;
using Skills;
using Units;
using UnityEngine;
using UserInterface;

namespace Shops
{
    public class ShopDeckMono_UI : MonoBehaviour
    {
        [SerializeField] private SlotDragAndDrop prefabSlot;

        [SerializeField] private ItemDragAndDrop prefabSkill;
        [SerializeField] private ItemDragAndDrop prefabRelic;

        [SerializeField] private Transform deckPlaceHolder;
        [SerializeField] private Transform relicPlaceHolder;
        [SerializeField] private List<PersonalInventory> portraits;
        [SerializeField] private List<BattleHero> BattlesHeroes;
        private BattleHero actualHero;
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
                PlayerData.getInstance().Heroes[i].Spawn(BattlesHeroes[i]);
                portraits[i].Initialize(PlayerData.getInstance().Heroes[i]);
            }

            actualHero = BattlesHeroes[0];
            
            InitializeDisplay(0);
        }

        private void OnDisable()
        {
            onHeroChange.EventListeners -= ChangeHero;
            onCampPointUsed.EventListeners -= InitializeDisplay;
        }


        private void InitializeDisplay(int empty)
        {
            ClearDecks();

            DeckMono Deck = GameObject.FindObjectOfType<DeckMono>();
            Deck.UpdateDeck();
            Deck.InitializeForCamp();
            
            foreach (SkillSO _skillSO in Deck.DrawPile)
            {
                GameObject _cell = GameObject.Instantiate(prefabSlot.gameObject, deckPlaceHolder);
                _cell.GetComponent<SlotDragAndDrop>().cellType = SlotDragAndDrop.CellType.DragOnly;
                _cell.GetComponent<SlotDragAndDrop>().unlimitedSource = true;
                _cell.GetComponent<SlotDragAndDrop>().containType = SlotDragAndDrop.ContainType.Skill;

                GameObject pref = GameObject.Instantiate(prefabSkill.gameObject, _cell.transform);
                pref.GetComponent<SkillInfo>().skill = Skill.CreateSkill(_skillSO, Deck, actualHero);
                pref.GetComponent<SkillInfo>().Unit = actualHero;
                pref.GetComponent<SkillInfo>().DisplayIcon();

                allSkills.Add(pref.GetComponent<SkillInfo>());
            }

            foreach (RelicSO _relicSO in Deck.Relics)
            {
                GameObject _cell = GameObject.Instantiate(prefabSlot.gameObject, relicPlaceHolder);
                _cell.GetComponent<SlotDragAndDrop>().cellType = SlotDragAndDrop.CellType.DragOnly;
                _cell.GetComponent<SlotDragAndDrop>().unlimitedSource = true;
                _cell.GetComponent<SlotDragAndDrop>().containType = SlotDragAndDrop.ContainType.Relic;

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

        private void ChangeHero(int _index)
        {
            DeckMono Deck = GameObject.FindObjectOfType<DeckMono>();
            actualHero = BattlesHeroes[_index];
            foreach (SkillInfo _skill in allSkills)
            {
                SkillSO s = _skill.skill.BaseSkill;
                _skill.skill = Skill.CreateSkill(s, Deck, actualHero);
            }
        }
    }
}