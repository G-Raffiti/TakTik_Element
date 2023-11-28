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
using UnityEngine.Serialization;
using UnityEngine.UI;
using UserInterface;

namespace Score
{
    public class ScoreSceneUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [FormerlySerializedAs("BossSpawnPlace")]
        [SerializeField] private List<Cell> bossSpawnPlace;
        
        [FormerlySerializedAs("Inventories")]
        [Header("Menu Panels")]
        [SerializeField] private List<PersonalInventory> inventories;
        [FormerlySerializedAs("RelicHolder")]
        [SerializeField] private List<Transform> relicHolder;
        [SerializeField] private Hero blanc;
        [FormerlySerializedAs("DrawPile")]
        [SerializeField] private Transform drawPile;
        [SerializeField] private BattleHero hero;
        private DeckMono deck;
        private bool isWin;
        
        [FormerlySerializedAs("DamageDealtTotal")]
        [Header("Text Fields in Stat Panel")]
        [SerializeField] private TextMeshProUGUI damageDealtTotal;
        [FormerlySerializedAs("DamageDealtBiggest")]
        [SerializeField] private TextMeshProUGUI damageDealtBiggest;
        [FormerlySerializedAs("DamageTakenTotal")]
        [SerializeField] private TextMeshProUGUI damageTakenTotal;
        [FormerlySerializedAs("DamageTakenBiggest")]
        [SerializeField] private TextMeshProUGUI damageTakenBiggest;
        [FormerlySerializedAs("CellWalked")]
        [SerializeField] private TextMeshProUGUI cellWalked;
        [FormerlySerializedAs("GearSalvaged")]
        [SerializeField] private TextMeshProUGUI gearSalvaged;
        [FormerlySerializedAs("CraftingMaterialCollected")]
        [SerializeField] private TextMeshProUGUI craftingMaterialCollected;

        [FormerlySerializedAs("ScorePoints")]
        [Header("Progress Bar")]
        [SerializeField] private TextMeshProUGUI scorePoints;
        [SerializeField] private TextMeshProUGUI gamePoints;
        [FormerlySerializedAs("ProgressBarFill")]
        [SerializeField] private Image progressBarFill;
        
        [FormerlySerializedAs("SkillPrefab")]
        [Header("Prefabs")]
        [SerializeField] private SkillInfo skillPrefab;
        [FormerlySerializedAs("SlotPrefab")]
        [SerializeField] private SlotDragAndDrop slotPrefab;
        [FormerlySerializedAs("RelicPrefab")]
        [SerializeField] private RelicInfo relicPrefab;
        
        private void Start()
        {
            BattleStage.EndGame();
            isWin = PlayerData.GetInstance().IsVictory;
            title.text = isWin ? "Victory !" : "Game Over !";
            for (int _i = 0; _i < ScoreHolder.Bosses.Count; _i++)
            {
                ScoreHolder.Bosses[_i].Spawn(bossSpawnPlace[_i]);
            }

            InitializeInventory();
            
            deck = GameObject.Find("DeckMono/Deck1").GetComponent<DeckMono>();
            hero.Spawn(blanc);

            InitializeDeck(deck.drawPile, hero, drawPile);

            InitializeStats();

            StartCoroutine(ProgressBar());
        }

        private void InitializeStats()
        {
            damageDealtTotal.text = $"Total Damage: {ScoreHolder.DamageDealtTotal}";
            damageDealtBiggest.text = $"Biggest Hit: {ScoreHolder.DamageDealtBiggest}";
            damageTakenTotal.text = $"Total Damage Taken: {ScoreHolder.DamageTakenTotal}";
            damageTakenBiggest.text = $"Biggest Hit Taken: {ScoreHolder.DamageTakenBiggest}";
            cellWalked.text = $"Distance walked: {ScoreHolder.CellWalked}";
            gearSalvaged.text = $"Gear destroyed in the Forge: {ScoreHolder.GearSalvaged}";
            craftingMaterialCollected.text = $"Crafting Material Collected: {ScoreHolder.CraftingMaterialCollected}";

        }

        private void InitializeInventory()
        {
            for (int _i = 0; _i < PlayerData.GetInstance().Heroes.Count; _i++)
            {
                inventories[_i].Initialize(PlayerData.GetInstance().Heroes[_i]);
                inventories[_i].FillInventory();
                
                foreach (RelicSo _relic in PlayerData.GetInstance().Heroes[_i].Relics)
                {
                    GameObject _relicObj = Instantiate(relicPrefab.gameObject, relicHolder[_i]);
                    _relicObj.GetComponent<RelicInfo>().CreateRelic(_relic);
                    _relicObj.GetComponent<RelicInfo>().DisplayIcon();
                }
            }
        }
        
        /// <summary>
        /// method <c>Set Pile</c>, Initialize the Deck View
        /// </summary>
        private void InitializeDeck(List<SkillSo> _pile, Unit _user, Transform _holder)
        {
            foreach (SkillSo _skillSo in _pile)
            {
                GameObject _slotsObj = Instantiate(slotPrefab.gameObject, _holder);
                _slotsObj.GetComponent<SlotDragAndDrop>().containType = SlotDragAndDrop.ContainType.Skill;
                _slotsObj.GetComponent<SlotDragAndDrop>().cellType = SlotDragAndDrop.CellType.DropOnly;
                
                GameObject _skillObj = Instantiate(skillPrefab.gameObject, _slotsObj.transform);
                _skillObj.GetComponent<SkillInfo>().skill = Skill.CreateSkill(_skillSo, deck, _user);
                _skillObj.GetComponent<SkillInfo>().unit = _user;
                _skillObj.GetComponent<SkillInfo>().DisplayIcon();
                
                _slotsObj.GetComponent<SlotDragAndDrop>().UpdateMyItem();
                _slotsObj.GetComponent<SlotDragAndDrop>().UpdateBackgroundState();
            }
        }

        private IEnumerator ProgressBar()
        {
            int _score = ScoreHolder.GameScore();
            int _progressScore = 0;
            progressBarFill.fillAmount = 0;
            string _point = "Score: ";
            scorePoints.text = $"{_point}{_progressScore}";
            gamePoints.text = String.Empty;

            for (int _i = 0; _i < _score; _i++)
            {
                _progressScore++;
                scorePoints.text = $"{_point}{_progressScore}";
                progressBarFill.fillAmount = (float) _progressScore / _score;
                yield return new WaitForSeconds(0.05f);
            }

            gamePoints.text = $"{(int)_score / 500f}";
        }

        public void Btn_ExitGame()
        {
            Application.Quit();
        }
        
        
    }
}