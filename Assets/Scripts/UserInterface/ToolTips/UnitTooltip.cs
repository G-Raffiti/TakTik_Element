using System;
using System.Collections.Generic;
using Buffs;
using Gears;
using Relics;
using Stats;
using TMPro;
using UISetupState;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UserInterface.ToolTips
{
    public class UnitTooltip : Tooltip
    {
        [FormerlySerializedAs("Unit")]
        [HideInInspector] public Unit unit;
        
        [Header("Unity References")]
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private Image unitSprite;
        [FormerlySerializedAs("HPTxt")]
        [SerializeField] private TextMeshProUGUI hpTxt;
        [FormerlySerializedAs("HPFill")]
        [SerializeField] private Image hpFill;
        [FormerlySerializedAs("ShieldTxt")]
        [SerializeField] private TextMeshProUGUI shieldTxt;
        [FormerlySerializedAs("ShieldFill")]
        [SerializeField] private Image shieldFill;
        [FormerlySerializedAs("WaterTxt")]
        [SerializeField] private TextMeshProUGUI waterTxt;
        [FormerlySerializedAs("WaterFill")]
        [SerializeField] private Image waterFill;
        [FormerlySerializedAs("NatureTxt")]
        [SerializeField] private TextMeshProUGUI natureTxt;
        [FormerlySerializedAs("NatureFill")]
        [SerializeField] private Image natureFill;
        [FormerlySerializedAs("FireTxt")]
        [SerializeField] private TextMeshProUGUI fireTxt;
        [FormerlySerializedAs("FireFill")]
        [SerializeField] private Image fireFill;
        [FormerlySerializedAs("PowerTxt")]
        [SerializeField] private TextMeshProUGUI powerTxt;
        [FormerlySerializedAs("PowerFill")]
        [SerializeField] private Image powerFill;
        [FormerlySerializedAs("SpeedTxt")]
        [SerializeField] private TextMeshProUGUI speedTxt;
        [FormerlySerializedAs("FocusTxt")]
        [SerializeField] private TextMeshProUGUI focusTxt;
        [FormerlySerializedAs("APTxt")]
        [SerializeField] private TextMeshProUGUI apTxt;
        [FormerlySerializedAs("MPTxt")]
        [SerializeField] private TextMeshProUGUI mpTxt;
        [FormerlySerializedAs("MainElementTxt")]
        [SerializeField] private TextMeshProUGUI mainElementTxt;
        [FormerlySerializedAs("OtherElementTxt")]
        [SerializeField] private TextMeshProUGUI otherElementTxt;
        [FormerlySerializedAs("RangeTxt")]
        [SerializeField] private TextMeshProUGUI rangeTxt;
        [FormerlySerializedAs("StatsBlock")]
        [SerializeField] protected GameObject statsBlock;
        [SerializeField] private List<Image> frames;
        [SerializeField] private Transform buffsHolder;
        
        [Header("Prefabs")] 
        [SerializeField] protected RelicInfo relicPref;
        [SerializeField] protected GearInfo gearPref;
        [SerializeField] private BuffInfo buffPref;
        
        protected override void ShowToolTip()
        {
            statsBlock.SetActive(UIManager.CompleteStats);
            nameTxt.text = unit.GetInfoMain();
            unitSprite.sprite = unit.GetIcon();
            hpTxt.text = $"{unit.battleStats.hp}/{unit.Total.hp}";
            hpFill.fillAmount = unit.battleStats.hp / (float) unit.Total.hp;
            shieldTxt.text = $"{unit.battleStats.shield}";
            shieldFill.fillAmount = unit.battleStats.shield / (float) Mathf.Max(unit.Total.hp, unit.battleStats.shield);
            waterTxt.text = $"{(int) unit.battleStats.affinity.water}";
            fireTxt.text = $"{(int) unit.battleStats.affinity.fire}";
            natureTxt.text = $"{(int) unit.battleStats.affinity.nature}";
            powerTxt.text = $"{unit.battleStats.power}";
            SetDamageBar();
            speedTxt.text = $"{unit.battleStats.speed}<sprite name=Speed>";
            focusTxt.text = $"{unit.battleStats.focus}<sprite name=Focus>";
            apTxt.text = $"{unit.battleStats.ap}";
            mpTxt.text = $"{unit.battleStats.mp}";
            mainElementTxt.text = unit.GetInfoDown();
            otherElementTxt.text = unit.GetElements();
            rangeTxt.text = unit.GetInfoRight();
            frames.ForEach(_i => _i.color = unit.GetTeamColor());

            foreach (Buff _buff in unit.Buffs)
            {
                GameObject _pref = Instantiate(buffPref.gameObject, buffsHolder);
                _pref.GetComponent<BuffInfo>().Unit = unit;
                _pref.GetComponent<BuffInfo>().Buff = _buff;
                _pref.GetComponent<BuffInfo>().DisplayIcon();
            }
        }

        private void SetDamageBar()
        {
            float[] _values = 
                {
                    unit.battleStats.GetPower(EElement.Fire), 
                    unit.battleStats.GetPower(EElement.Water), 
                    unit.battleStats.GetPower(EElement.Nature), 
                    unit.battleStats.GetPower(EElement.None)
                };
            float _max = Mathf.Max(_values);
            float _min = Mathf.Min(_values);
            if (_min < 0)
            {
                _max += Mathf.Abs(_min);
            }
            else _min = 0;
            fireFill.fillAmount = (unit.battleStats.affinity.fire - _min) / _max;
            waterFill.fillAmount = (unit.battleStats.affinity.water - _min) / _max;
            natureFill.fillAmount = (unit.battleStats.affinity.nature - _min) / _max;
            powerFill.fillAmount = (unit.battleStats.power - _min) / _max;
        }

        public override void HideTooltip()
        {
            while (buffsHolder.childCount > 0)
            {
                DestroyImmediate(buffsHolder.GetChild(0).gameObject);
            }
            base.HideTooltip();
        }

        protected override Vector3 Position()
        {
            Vector3 _newPos = Camera.main.WorldToScreenPoint(unit.transform.position) + offset;
            
            float _rightEdgeToScreenEdgeDistance = Screen.width - (_newPos.x + backgroundRectTransform.rect.width * popupCanvas.scaleFactor) - Padding.x;
            if (_rightEdgeToScreenEdgeDistance < 0)
            {
                _newPos.x += _rightEdgeToScreenEdgeDistance;
            }

            float _topEdgeToScreenEdgeDistance = Screen.height - (_newPos.y + backgroundRectTransform.rect.height * popupCanvas.scaleFactor) - Padding.y;
            if (_topEdgeToScreenEdgeDistance < 0)
            {
                _newPos.y += _topEdgeToScreenEdgeDistance;
            }
            
            return _newPos;
        }
    }
}