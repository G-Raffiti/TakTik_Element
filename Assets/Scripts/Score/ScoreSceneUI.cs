using System;
using System.Collections;
using System.Collections.Generic;
using _DragAndDropSystem;
using _Instances;
using Cells;
using Decks;
using Relics;
using Skills;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;
using UserInterface;

namespace Score
{
    public class ScoreSceneUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private List<Cell> BossSpawnPlace;
        
        [Header("Menu Panels")]
        [SerializeField] private List<PersonalInventory> Inventories;
        [SerializeField] private List<Transform> RelicHolder;
        [SerializeField] private Hero blanc;
        [SerializeField] private Transform DrawPile;
        [SerializeField] private BattleHero hero;
        private DeckMono deck;
        private bool isWin;
        
        [Header("Text Fields in Stat Panel")]
        [SerializeField] private TextMeshProUGUI DamageDealtTotal;
        [SerializeField] private TextMeshProUGUI DamageDealtBiggest;
        [SerializeField] private TextMeshProUGUI DamageTakenTotal;
        [SerializeField] private TextMeshProUGUI DamageTakenBiggest;
        [SerializeField] private TextMeshProUGUI CellWalked;
        [SerializeField] private TextMeshProUGUI GearSalvaged;
        [SerializeField] private TextMeshProUGUI CraftingMaterialCollected;

        [Header("Progress Bar")]
        [SerializeField] private TextMeshProUGUI ScorePoints;
        [SerializeField] private TextMeshProUGUI gamePoints;
        [SerializeField] private Image ProgressBarFill;
        
        [Header("Prefabs")]
        [SerializeField] private SkillInfo SkillPrefab;
        [SerializeField] private SlotDragAndDrop SlotPrefab;
        [SerializeField] private RelicInfo RelicPrefab;
        
        private void Start()
        {
            BattleStage.EndGame();
            isWin = PlayerData.getInstance().IsVictory;
            title.text = isWin ? "Victory !" : "Game Over !";
            for (int i = 0; i < ScoreHolder.Bosses.Count; i++)
            {
                ScoreHolder.Bosses[i].Spawn(BossSpawnPlace[i]);
            }

            InitializeInventory();
            
            deck = GameObject.Find("DeckMono/Deck1").GetComponent<DeckMono>();
            hero.Spawn(blanc);

            InitializeDeck(deck.DrawPile, hero, DrawPile);

            InitializeStats();

            StartCoroutine(ProgressBar());
        }

        private void InitializeStats()
        {
            DamageDealtTotal.text = $"Total Damage: {ScoreHolder.DamageDealtTotal}";
            DamageDealtBiggest.text = $"Biggest Hit: {ScoreHolder.DamageDealtBiggest}";
            DamageTakenTotal.text = $"Total Damage Taken: {ScoreHolder.DamageTakenTotal}";
            DamageTakenBiggest.text = $"Biggest Hit Taken: {ScoreHolder.DamageTakenBiggest}";
            CellWalked.text = $"Distance walked: {ScoreHolder.CellWalked}";
            GearSalvaged.text = $"Gear destroyed in the Forge: {ScoreHolder.GearSalvaged}";
            CraftingMaterialCollected.text = $"Crafting Material Collected: {ScoreHolder.CraftingMaterialCollected}";

        }

        private void InitializeInventory()
        {
            for (int i = 0; i < PlayerData.getInstance().Heroes.Count; i++)
            {
                Inventories[i].Initialize(PlayerData.getInstance().Heroes[i]);
                Inventories[i].FillInventory();
                
                foreach (RelicSO _relic in PlayerData.getInstance().Heroes[i].Relics)
                {
                    GameObject relicObj = Instantiate(RelicPrefab.gameObject, RelicHolder[i]);
                    relicObj.GetComponent<RelicInfo>().CreateRelic(_relic);
                    relicObj.GetComponent<RelicInfo>().DisplayIcon();
                }
            }
        }
        
        /// <summary>
        /// method <c>Set Pile</c>, Initialize the Deck View
        /// </summary>
        private void InitializeDeck(List<SkillSO> Pile, Unit user, Transform holder)
        {
            foreach (SkillSO _skillSO in Pile)
            {
                GameObject SlotsObj = Instantiate(SlotPrefab.gameObject, holder);
                SlotsObj.GetComponent<SlotDragAndDrop>().containType = SlotDragAndDrop.ContainType.Skill;
                SlotsObj.GetComponent<SlotDragAndDrop>().cellType = SlotDragAndDrop.CellType.DropOnly;
                
                GameObject SkillObj = Instantiate(SkillPrefab.gameObject, SlotsObj.transform);
                SkillObj.GetComponent<SkillInfo>().skill = Skill.CreateSkill(_skillSO, deck, user);
                SkillObj.GetComponent<SkillInfo>().Unit = user;
                SkillObj.GetComponent<SkillInfo>().DisplayIcon();
                
                SlotsObj.GetComponent<SlotDragAndDrop>().UpdateMyItem();
                SlotsObj.GetComponent<SlotDragAndDrop>().UpdateBackgroundState();
            }
        }

        private IEnumerator ProgressBar()
        {
            int score = ScoreHolder.GameScore();
            int progressScore = 0;
            ProgressBarFill.fillAmount = 0;
            string point = "Score: ";
            ScorePoints.text = $"{point}{progressScore}";
            gamePoints.text = String.Empty;

            for (int i = 0; i < score; i++)
            {
                progressScore++;
                ScorePoints.text = $"{point}{progressScore}";
                ProgressBarFill.fillAmount = (float) progressScore / score;
                yield return new WaitForSeconds(0.05f);
            }

            gamePoints.text = $"{(int)score / 500f}";
        }

        public void Btn_ExitGame()
        {
            Application.Quit();
        }
        
        
    }
}