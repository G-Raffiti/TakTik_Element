using System.Collections.Generic;
using _DragAndDropSystem;
using _EventSystem.CustomEvents;
using _Extension;
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

        [SerializeField] private Transform deckPlaceHolder;
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
                GameObject _slot = GameObject.Instantiate(prefabSlot.gameObject, deckPlaceHolder);
                _slot.GetComponent<SlotDragAndDrop>().cellType = SlotDragAndDrop.CellType.DragOnly;
                _slot.GetComponent<SlotDragAndDrop>().unlimitedSource = true;
                _slot.GetComponent<SlotDragAndDrop>().containType = SlotDragAndDrop.ContainType.Skill;

                GameObject pref = GameObject.Instantiate(prefabSkill.gameObject, _slot.transform);
                pref.GetComponent<SkillInfo>().skill = Skill.CreateSkill(_skillSO, Deck, actualHero);
                pref.GetComponent<SkillInfo>().Unit = actualHero;
                pref.GetComponent<SkillInfo>().DisplayIcon();
                
                _slot.GetComponent<SlotDragAndDrop>().UpdateMyItem();
                _slot.GetComponent<SlotDragAndDrop>().UpdateBackgroundState();

                allSkills.Add(pref.GetComponent<SkillInfo>());
            }
        }

        private void ClearDecks()
        {
            
            deckPlaceHolder.Clear();

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