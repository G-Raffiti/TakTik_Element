using Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.ToolTips
{
    public class SkillTooltip : Tooltip
    {
        [HideInInspector]
        public SkillInfo Skill;

        [SerializeField] private TextMeshProUGUI Name;
        [SerializeField] private Image elementIcon;
        [SerializeField] private TextMeshProUGUI cost;
        [SerializeField] private TextMeshProUGUI costShadow;
        [SerializeField] private Image illustration;
        [SerializeField] private TextMeshProUGUI effectTxt;
        [SerializeField] private TextMeshProUGUI rangeTxt;
        [SerializeField] private TextMeshProUGUI buffsTxt;
        [SerializeField] private Image frame;
        [SerializeField] private Image shadowEffect;

        protected override void ShowToolTip()
        {
            Name.text = Skill.skill.BaseSkill.Name;
            elementIcon.sprite = Skill.skill.Element.Icon;
            cost.text = $"{Skill.skill.Cost}";
            costShadow.text = $"{Skill.skill.Cost}";
            illustration.sprite = Skill.GetIcon();
            effectTxt.text = Skill.GetInfoLeft();
            rangeTxt.text = Skill.GetInfoRight();
            buffsTxt.text = Skill.GetInfoDown();
            frame.color = Skill.skill.Element.TextColour;
            shadowEffect.sprite = Skill.skill.BaseSkill.Monster != null ? Skill.skill.BaseSkill.Monster.UnitSprite : null;
        }
    }
}