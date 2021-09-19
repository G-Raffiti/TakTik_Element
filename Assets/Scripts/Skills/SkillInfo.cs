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
using Range = Stats.Range;

namespace Skills
{
    /// <summary>
    /// Class to attach to a Button that will give access to a Deck to Use Skills
    /// </summary>
    public class SkillInfo : InfoBehaviour
    {
        [SerializeField] private VoidEvent OnSkillUsed;
        [SerializeField] private InfoEvent newSkill;
        
        public SkillSO Skill { get; private set; }
        public Unit Unit { get; private set; }
        [FormerlySerializedAs("Deck")] [SerializeField] private Deck deck;
        public Deck Deck => deck;
        public Range Range { get; private set; }
        public Power Power { get; private set; }
        public Element Element { get; private set; }
        public EAffect Affect { get; private set; }
        public int Cost { get; private set; }
        public ESkill Type { get; private set; }
        public List<Buff> Buffs { get; private set; }
        
        public void CreateSkill(Unit _unit)
        {
            Skill = deck.ActualSkill;
            Unit = _unit;
            if (Skill.Range.CanBeModified)
                Range = deck.Range + Unit.BattleStats.Range;
            else Range = Skill.Range;
            Power = deck.Power + Unit.BattleStats.Power;
            Element = deck.Element;
            Affect = deck.Affect;
            Cost = 1;
            Type = Skill.Type;
            newSkill?.Raise(this);
            Buffs = new List<Buff>();
            deck.StatusEffects.ForEach(status => Buffs.Add(new Buff(Unit, status)));
        }

        public void CreateSkillForPlayingUnit()
        {
            CreateSkill(BattleStateManager.instance.PlayingUnit);
        }
        
        public void UseSkill(Cell cell)
        {
            if (Unit.BattleStats.AP < Cost) return;
            if (!deck.UseSkill(this, cell)) return;
            
            Unit.BattleStats.AP -= Cost;
            deck.ShuffleDeck();
            CreateSkill(BattleStateManager.instance.PlayingUnit);
            
            BattleStateManager.instance.OnSkillUsed();
        }

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
            deck.Effects.ForEach(effect => str += effect.InfoEffect(this) + "\n");
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
            if (Unit.BattleStats.AP >= 1)
                BattleStateManager.instance.BattleState = new BattleStateSkillSelected(BattleStateManager.instance, this);
        }

        #endregion
    }
}