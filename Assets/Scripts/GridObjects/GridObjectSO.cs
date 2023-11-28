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
    public abstract class GridObjectSo : ScriptableObject, IInfo
    {
        [SerializeField] private EGridObject type;
        [SerializeField] private bool movable;
        [SerializeField] private Sprite image;
        [SerializeField] private new string name;
        
        public virtual Sprite Image => image;
        public bool Movable => movable;
        public EGridObject Type => type;
        public virtual string Name => name;

        public virtual void Interact(Unit _actor, Cell _location)
        {
            if (!GetZoneOfInteraction(_location).Contains(_actor.Cell)) return;
        }

        public virtual List<Cell> GetZoneOfInteraction(Cell _location)
        {
            return Zone.GetRange(new GridRange(EZone.Contact, EZone.Basic, 1, 0), _location).ToList();
        }

        public virtual void MarkAsInteractable(Cell _location)
        {
            _location.MarkAsReachable();
        }

        public virtual void ShowAction(Unit _actor, Cell _location)
        {
            if (!GetZoneOfInteraction(_location).Contains(_actor.Cell)) return;
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