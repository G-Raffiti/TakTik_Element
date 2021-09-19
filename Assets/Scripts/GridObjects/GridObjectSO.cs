using System.Collections.Generic;
using System.Linq;
using Cells;
using Resources.ToolTip.Scripts;
using Skills._Zone;
using Stats;
using Units;
using UnityEngine;

namespace GridObjects
{
    public enum EGridObject {Tree, Stone, LootBox}
    public abstract class GridObjectSO : ScriptableObject, IInfo
    {
        [SerializeField] private EGridObject type;
        [SerializeField] private bool movable;
        [SerializeField] private Sprite image;
        [SerializeField] private new string name;
        
        public Sprite Image => image;
        public bool Movable => movable;
        public EGridObject Type => type;
        public virtual string Name => name;

        public virtual void Interact(Unit actor, Cell location)
        {
            if (!GetZoneOfInteraction(location).Contains(actor.Cell)) return;
        }

        public virtual List<Cell> GetZoneOfInteraction(Cell location)
        {
            return Zone.GetRange(new Range(EZone.Contact, EZone.Basic, 1, 0), location).ToList();
        }

        public virtual void MarkAsInteractable(Cell location)
        {
            location.MarkAsReachable();
        }

        #region IInfo

        public virtual string GetInfoMain()
        {
            return ColouredName();
        }

        public virtual string GetInfoLeft() { return ""; }

        public virtual string GetInfoRight() { return ""; }

        public virtual string GetInfoDown() { return ""; }

        public Sprite GetIcon()
        {
            return image;
        }

        public virtual string ColouredName()
        {
            return $"{type}";
        }

    #endregion
    }
}