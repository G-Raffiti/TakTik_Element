using System.Collections.Generic;
using _EventSystem.CustomEvents;
using _ScriptableObject;
using Cells;
using Grid;
using Grid.GridStates;
using Resources.ToolTip.Scripts;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Stats;
using StatusEffect;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Range = Stats.Range;

namespace Skills
{
    /// <summary>
    /// Class to attach to a Button that will give access to a Deck to Use Skills
    /// </summary>
    public class SkillInfo : InfoBehaviour
    {
        [SerializeField] private bool Clickable;
        [SerializeField] private Image icon;
        [SerializeField] private Deck deck;
        
        [Header("Event Sender")] 
        [SerializeField] private SkillEvent onSkillSelected;
        [SerializeField] private VoidEvent OnSkillUsed;
        
        public SkillSO Skill { get; private set; }
        public Unit Unit { get; private set; }
        public Deck Deck
        {
            get => deck;
            set => deck = value;
        }

        public Range Range { get; private set; }
        public Power Power { get; private set; }
        public Element Element { get; private set; }
        public EAffect Affect { get; private set; }
        public int Cost { get; private set; }
        public ESkill Type { get; private set; }
        public List<Buff> Buffs { get; private set; }

        public void UpdateSkill(SkillSO _skillSO, Unit _unit)
        {
            deck.UpdateSkill(_skillSO);
            Skill = _skillSO;
            Unit = _unit;
            if (Skill.Range.CanBeModified)
                Range = deck.Range + Unit.BattleStats.Range;
            else Range = Skill.Range;
            Power = deck.Power + Unit.BattleStats.Power;
            Element = deck.Element;
            Affect = deck.Affect;
            Cost = deck.Cost;
            Type = Skill.Type;
            Buffs = new List<Buff>();
            deck.StatusEffects.ForEach(status => Buffs.Add(new Buff(Unit, status)));
        }
        public void UpdateSkill(int index, Unit _unit)
        {
            UpdateSkill(deck.Skills[index], _unit);
        }
        
        /// <summary>
        /// Method Called by the IA to Use a Skill that is not in the Deck
        /// </summary>
        /// <param name="skill">
        /// the skill to Use
        /// </param>
        /// <param name="cell">
        /// the targeted Cell
        /// </param>
        /// <param name="sender">
        /// the Unit who use the skill
        /// </param>
        public void UseSkill(SkillSO skill, Cell cell, Unit sender)
        {
            UpdateSkill(skill, sender);
            UseSkill(cell);
            Deck.NextSkill();
            UpdateSkill(0, Unit);
        }
        
        /// <summary>
        /// Method Called buy the Player by Clicking in the Icon in BattleScene
        /// </summary>
        /// <param name="cell"></param>
        public void UseSkill(Cell cell)
        {
            if (Unit.BattleStats.AP < Cost) return;
            if (!deck.UseSkill(this, cell)) return;

            Unit.BattleStats.AP -= Cost;

            OnSkillUsed?.Raise();
        }

        /// <summary>
        /// Return the Zone to Highlight to show the cell touched by the Actual Skill
        /// </summary>
        /// <param name="_cell"></param>
        /// <returns></returns>
        public List<Cell> GetZoneOfEffect(Cell _cell)
        {
            return Zone.GetZone(Range, _cell);
        }
        
        #region IInfo
        public override string GetInfoMain()
        {
            return $"{ColouredName()}\n{Skill.Type} of {Element.Name}\ncost: {Cost} <sprite name=AP>";
        }

        public override string GetInfoLeft()
        {
            string str = "";
            Skill.Effects.ForEach(effect => str += effect.InfoEffect(this) + "\n");
            return str;
        }

        public override string GetInfoRight()
        {
            return Range.ToString();
        }

        public override string GetInfoDown()
        {
            string str = "";
            Buffs.ForEach(buff => str += buff.InfoBuff());
            return str;
        }

        public override Sprite GetIcon()
        {
            return Skill.Icon;
        }

        public override string ColouredName()
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"<size=35><color=#{_hexColor}>{Skill.Name}</color></size>";
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!Clickable) return;
            if (Unit.BattleStats.AP >= 1)
                onSkillSelected?.Raise(this);
        }
        
        public override void DisplayIcon()
        {
            if (icon != null)
                icon.sprite = GetIcon();
            EnableIcon();
        }

        public void EnableIcon()
        {
            if (icon == null) return;
            if ((BattleStateManager.instance.PlayingUnit.playerNumber == 0 && (int) BattleStateManager.instance.PlayingUnit.BattleStats.AP > 0) || !Clickable)
            {
                icon.color = Color.white;
            }
            else icon.color = Color.gray;
        }

        #endregion
    }
}