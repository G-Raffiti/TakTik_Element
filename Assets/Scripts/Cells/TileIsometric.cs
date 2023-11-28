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
            return $"{cellSo.Type}";
        }

        public string GetInfoLeft()
        {
            string _str = "";
            if (IsTaken)
            {
                _str += "the Cell is "; 
                _str += CurrentUnit ? $"Occupied by: {CurrentUnit.unitName}" : "not walkable";
                _str += CurrentGridObject ? $"Taken by: a {CurrentGridObject.GridObjectSo.Type}" : "not walkable";
            }

            if (IsCorrupted)
            {
                _str += "the Cell is Corrupted";
            }

            return _str;
        }

        public string GetInfoRight()
        {
            return "";
        }

        public string GetInfoDown()
        {
            string _str = "";
            foreach (Buff _buff in Buffs)
            {
                _str += $"{_buff.InfoBuffOnCell(this)}\n";
            }

            return _str;
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