using _ScriptableObject;
using Units;
using UnityEngine;

namespace StatusEffect
{
    public abstract class StatusSO : ScriptableObject
    {
        [SerializeField] private bool betweenTurn;
        [SerializeField] private Element element;
        [SerializeField] private Sprite onFloorSprite;
        [SerializeField] private new string name;
        public bool BetweenTurn => betweenTurn;
        public Element Element => element;
        public Sprite OnFloorSprite => onFloorSprite;

        public abstract void ActiveEffect(Buff _buff, Unit _unit);
        public abstract void PassiveEffect(Buff _buff, Unit _unit);
        public abstract void EndPassiveEffect(Buff _buff, Unit _unit);
        public abstract float GetBuffValue(Unit sender);
        public abstract int GetBuffDuration(Unit sender);
        public abstract string InfoEffect(Buff _buff);
        public abstract string InfoOnUnit(Buff _buff, Unit _unit);

        public virtual Buff AddBuff(Buff a, Buff b)
        {
            if (a.Effect != b.Effect) return a;
            Buff ret = new Buff(a);
            ret.Duration += b.Duration;
            ret.Value += b.Value;

            return ret;
        }

        public string Name => $"<color=#{ColorUtility.ToHtmlStringRGB(Element.TextColour)}>{name}</color>";

        public abstract string InfoOnFloor(Buff _buff);
    }
}