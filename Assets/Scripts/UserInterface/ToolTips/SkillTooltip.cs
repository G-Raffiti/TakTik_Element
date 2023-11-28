using Buffs;
using Skills;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UserInterface.ToolTips
{
    public class SkillTooltip : Tooltip
    {
        [FormerlySerializedAs("Skill")]
        [HideInInspector]
        public SkillInfo skill;

        [FormerlySerializedAs("Name")]
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private Image elementIcon;
        [SerializeField] private TextMeshProUGUI cost;
        [SerializeField] private Image illustration;
        [SerializeField] private TextMeshProUGUI effectTxt;
        [SerializeField] private TextMeshProUGUI rangeTxt;
        [SerializeField] private Image frame;
        [SerializeField] private Image illustrationFrame;
        [SerializeField] private Image shadowEffect;
        [SerializeField] private Transform buffsHolder;
        [SerializeField] private BuffInfo buffPref;

        protected override void ShowToolTip()
        {
            name.text = skill.skill.BaseSkill.Name;
            elementIcon.sprite = skill.skill.Element.Icon;
            cost.text = $"{skill.skill.Cost}";
            illustration.sprite = skill.GetIcon();
            effectTxt.text = skill.GetInfoLeft();
            rangeTxt.text = skill.GetInfoRight();
            frame.color = skill.skill.Element.TextColour;
            illustrationFrame.color = skill.skill.Element.TextColour;
            foreach (Buff _buff in skill.skill.Buffs)
            {
                GameObject _pref = Instantiate(buffPref.gameObject, buffsHolder);
                _pref.GetComponent<BuffInfo>().Buff = _buff;
                _pref.GetComponent<BuffInfo>().DisplayIcon();
            }
            if (skill.skill.BaseSkill.Monster != null)
            {
                shadowEffect.color = new Color(0,0,0,0.8f);
                shadowEffect.sprite = skill.skill.BaseSkill.Monster.UnitSprite;
            }
            else shadowEffect.color = Color.clear;
        }
        
        public override void HideTooltip()
        {
            while (buffsHolder.childCount > 0)
            {
                DestroyImmediate(buffsHolder.GetChild(0).gameObject);
            }
            
            base.HideTooltip();
        }

        protected override Vector3 LockPosition()
        {
            return lockPosition - backgroundRectTransform.rect.size / 2;
        }
    }
}