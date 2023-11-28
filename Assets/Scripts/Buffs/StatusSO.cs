using _ScriptableObject;
using Cells;
using Units;
using UnityEngine;

namespace Buffs
{
    public enum EBuff
    {
        Buff,
        Debuff,
    }
    public abstract class StatusSo : ScriptableObject
    {
        [SerializeField] private EBuff type;
        [SerializeField] private bool betweenTurn;
        [SerializeField] private Element element;
        [SerializeField] private Sprite onFloorSprite;
        [SerializeField] private string buffName;
        [SerializeField] protected int baseDuration;
        [SerializeField] protected bool isDefinitive;

        public EBuff Type => type;
        public bool BetweenTurn => betweenTurn;
        public Element Element => element;
        public Sprite OnFloorSprite => onFloorSprite;
        public string Name => $"<color=#{ColorUtility.ToHtmlStringRGB(Element.TextColour)}>{buffName}</color>";
        public bool IsDefinitive => isDefinitive;

        public abstract void ActiveEffect(Buff _buff, Unit _unit);
        public abstract void PassiveEffect(Buff _buff, Unit _unit);
        public abstract void EndPassiveEffect(Buff _buff, Unit _unit);
        public abstract float GetBuffValue(Unit _sender);
        
        /// <summary>
        /// Method called when a Unit Step by a Cell affected by this Buff
        /// </summary>
        public virtual void OnUnitTakeCell(Buff _buff, Unit _unit)
        {
            ActiveEffect(_buff, _unit);
        }
        
        public virtual int GetBuffDuration(Unit _sender)
        {
            return baseDuration + _sender.battleStats.GetFocus();
        }
        public abstract string InfoEffect(Buff _buff);
        public abstract string InfoOnUnit(Buff _buff, Unit _unit);

        public virtual Buff AddBuff(Buff _a, Buff _b)
        {
            if (_a.Effect != _b.Effect) return _a;
            Buff _ret = new Buff(_a);
            _ret.duration += _b.duration;
            _ret.value += _b.value;

            return _ret;
        }
        public abstract string InfoOnFloor(Cell _cell, Buff _buff);
    }
}