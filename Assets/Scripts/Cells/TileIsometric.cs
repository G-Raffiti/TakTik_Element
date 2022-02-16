using Buffs;
using Resources.ToolTip.Scripts;
using UnityEngine;

namespace Cells
{
    public class TileIsometric : Cell, IInfo
    {
        #region IInfo

        public string GetInfoMain()
        {
            return $"{cellSO.Type}";
        }

        public string GetInfoLeft()
        {
            string str = "";
            if (IsTaken)
            {
                str += "the Cell is "; 
                str += CurrentUnit ? $"Occupied by: {CurrentUnit.UnitName}" : "not walkable";
                str += CurrentGridObject ? $"Taken by: a {CurrentGridObject.GridObjectSO.Type}" : "not walkable";
            }

            if (isCorrupted)
            {
                str += "the Cell is Corrupted";
            }

            return str;
        }

        public string GetInfoRight()
        {
            return "";
        }

        public string GetInfoDown()
        {
            string str = "";
            foreach (Buff _buff in Buffs)
            {
                str += $"{_buff.InfoBuffOnCell(this)}\n";
            }

            return str;
        }

        public Sprite GetIcon()
        {
            return background.sprite;
        }

        public string ColouredName()
        {
            return "";
        }

    #endregion

    
    }
}