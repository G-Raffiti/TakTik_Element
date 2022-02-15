using System;
using _EventSystem.CustomEvents;
using _LeanTween.Framework;
using Buffs;
using StateMachine;
using TMPro;
using UISetupState;
using Units;
using UnityEngine;
using UnityEngine.UI;
using Void = _EventSystem.CustomEvents.Void;

namespace UserInterface
{
    public class LifeBar : MonoBehaviour
    {
        [Header("Unity References")]
        [SerializeField] private Image fillHP;
        [SerializeField] private Image fillShield;
        [SerializeField] private Transform buffsHolder;
        [SerializeField] private Unit unit;
        [SerializeField] private Canvas canvas;
        [SerializeField] private TextMeshProUGUI lifeTxt;
        [SerializeField] private TextMeshProUGUI shieldTxt;

        [Header("Prefab")]
        [SerializeField] private BuffInfo buffPrefab;
        
        [Header("Event Listener")]
        [SerializeField] private UnitEvent onUnitStartTurn;
        [SerializeField] private VoidEvent onSkillUsed;
        [SerializeField] private VoidEvent onActionDone;
        
        private int maxShield;

        private void Start()
        {
            lifeTxt.text = String.Empty;
            shieldTxt.text = String.Empty;
            
            onActionDone.EventListeners += Display;
            onSkillUsed.EventListeners += Display;
            onUnitStartTurn.EventListeners += Display;
            Hide();
        }

        private void OnDestroy()
        {
            onActionDone.EventListeners -= Display;
            onSkillUsed.EventListeners -= Display;
            onUnitStartTurn.EventListeners -= Display;
        }

        public void Initialize()
        {
            ClearBuffs();
            maxShield = unit.BattleStats.Shield;
            Display(new Void());
        }

        private void ClearBuffs()
        {
            while (buffsHolder.childCount > 0)
            {
                DestroyImmediate(buffsHolder.GetChild(0).gameObject);
            }
        }

        private void Display(Void empty)
        {
            lifeTxt.text = $"{unit.BattleStats.HP}/{unit.Total.HP}";
            shieldTxt.text = $"{unit.BattleStats.Shield}";
            
            if (unit.BattleStats.Shield > maxShield)
                maxShield = unit.BattleStats.Shield;
            
            fillHP.fillAmount = unit.BattleStats.HP / (float) unit.Total.HP;
            fillShield.fillAmount = unit.BattleStats.Shield / (float) maxShield;

            ClearBuffs();

            foreach (Buff _buff in unit.Buffs)
            {
                GameObject _pref = Instantiate(buffPrefab.gameObject, buffsHolder);
                _pref.GetComponent<BuffInfo>().Initialize(_buff, unit).DisplayIcon();
            }
        }

        private void Display(Unit u)
        {
            Display(new Void());
        }

        public void AutoSortOrder()
        {
            canvas.sortingOrder = 500 - (int)(unit.transform.position.y/0.577f);
        }

        public void Hide()
        {
            AutoSortOrder();
            LeanTween.scale(canvas.gameObject, Vector3.one * 0.01f, 0.1f);
            if (!UI_Manager.ActiveLifeBar)
                LeanTween.alphaCanvas(canvas.GetComponent<CanvasGroup>(), 0, 0.1f);
        }

        public void Activate(bool isActive)
        {
            if (isActive)
            {
                Display(new Void());
                LeanTween.alphaCanvas(canvas.GetComponent<CanvasGroup>(), 1, 0.1f);
                LeanTween.scale(canvas.gameObject, Vector3.one * 0.01f, 0.1f);
            }
            else
            {
                LeanTween.scale(canvas.gameObject, Vector3.one * 0.01f, 0.1f);
                LeanTween.alphaCanvas(canvas.GetComponent<CanvasGroup>(), 0, 0.1f);
            }
            
            AutoSortOrder();
        }

        public void Show()
        {
            Display(new Void());
            LeanTween.alphaCanvas(canvas.GetComponent<CanvasGroup>(), 1, 0.1f);
            LeanTween.scale(canvas.gameObject, Vector3.one * 0.012f, 0.2f);
            canvas.sortingOrder = 1000;
        }

        public void HideForced()
        {
            LeanTween.scale(canvas.gameObject, Vector3.one * 0.01f, 0.1f);
            LeanTween.alphaCanvas(canvas.GetComponent<CanvasGroup>(), 0, 0.1f);
            AutoSortOrder();
        }
    }
}