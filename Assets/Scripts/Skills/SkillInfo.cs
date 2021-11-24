using System.Collections.Generic;
using _EventSystem.CustomEvents;
using Cells;
using Resources.ToolTip.Scripts;
using Skills._Zone;
using Stats;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Skills
{
    /// <summary>
    /// Class to attach to a Button that will give access to a Deck to Use Skills
    /// </summary>
    public class SkillInfo : InfoBehaviour, IPointerClickHandler
    {
        [SerializeField] private bool Clickable;
        [SerializeField] private Image icon;
        [SerializeField] private Deck deck;
        
        [Header("Event Sender")] 
        [SerializeField] private SkillEvent onSkillSelected;
        
        public Skill Skill { get; private set; }

        public void UpdateSkill(Skill _skill)
        {
            Skill = _skill;
        }
        public void UpdateSkill(int index)
        {
            UpdateSkill(deck.UsableSkills[index]);
        }

        /// <summary>
        /// Return the Zone to Highlight to show the cell touched by the Actual Skill
        /// </summary>
        /// <param name="_cell"></param>
        /// <returns></returns>
        public List<Cell> GetZoneOfEffect(Cell _cell)
        {
            return Zone.GetZone(Skill.Range, _cell);
        }

        public List<Cell> GetRangeFrom(Cell _cell)
        {
            return Skill.Range.NeedView ? Zone.CellsInView(this, _cell) : Zone.CellsInRange(this, _cell);
        }
        
        #region IInfo
        public override string GetInfoMain()
        {
            return $"{ColouredName()}\n{Skill.Type} of {Skill.Element.Name}\ncost: {Skill.Cost} <sprite name=AP>";
        }

        public override string GetInfoLeft()
        {
            string str = "";
            Skill.Effects.ForEach(effect => str += effect.InfoEffect(this) + "\n");
            return str;
        }

        public override string GetInfoRight()
        {
            return Skill.Range.ToString();
        }

        public override string GetInfoDown()
        {
            string str = "";
            Skill.Buffs.ForEach(buff => str += buff.InfoBuff());
            return str;
        }

        public override Sprite GetIcon()
        {
            return Skill.BaseSkill.Icon;
        }

        public override string ColouredName()
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Skill.Element.TextColour);
            return $"<color=#{_hexColor}>{Skill.BaseSkill.Name}</color>";
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!Clickable) return;
            if (Skill.Unit.BattleStats.AP >= Skill.Cost)
                onSkillSelected?.Raise(this);
        }
        
        public override void DisplayIcon()
        {
            if (icon != null)
                icon.sprite = GetIcon();
        }

        #endregion

        public int GetPower(EElement _elementType)
        {
            return Skill.Power;
        }
    }
}