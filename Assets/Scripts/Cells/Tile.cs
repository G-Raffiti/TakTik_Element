using System;
using System.Collections.Generic;
using _SaveSystem;
using Grid;
using GridObjects;
using Resources.ToolTip.Scripts;
using StatusEffect;
using Units;
using UnityEngine;
using UserInterface;

namespace Cells
{
    public enum ETile
    {
        Empty,
        Tree,
        Lava,
        Void,
        Spawn,
    }
    public class Tile : Square, IInfo
    {
        [SerializeField] private ETile cellType;
        [SerializeField] private ColorSet colorSet;
        [SerializeField] private List<StatusSO> permaBuffs;
        [SerializeField] private int power;
        [SerializeField] private Sprite full;

        public override bool isCorrupted { get; set; }
        
        private List<Buff> buffs;
        public override ETile CellType => cellType;
        public override List<Buff> Buffs => buffs;
        public override int Power => power;

        [Header("Unity Infos")]
        [SerializeField] private SpriteRenderer frame;
        [SerializeField] private SpriteRenderer fade;
        [SerializeField] public SpriteRenderer element;
        [SerializeField] private SpriteRenderer effect;
        [SerializeField] private SpriteRenderer background;
        
        private Dictionary<EColor, Color> Colors = new Dictionary<EColor, Color>();

        public void Start()
        {
            isCorrupted = false;
            buffs = new List<Buff>();
            Colors = colorSet.GetColors();
            element.sortingOrder = 100 - (int)transform.position.y;
            foreach (StatusSO _status in permaBuffs)
            {
                buffs.Add(new Buff(this, _status));
                effect.sprite = _status.OnFloorSprite;
            }
        }
        
        public override Vector3 GetCellDimensions()
        {
            return new Vector3(1,1,0);
        }
        
        public override void AddBuff(Buff _buff)
        {
            Buff buff = new Buff(_buff);
            buff.Duration *= BattleStateManager.instance.Units.Count;
            
            bool applied = false;
            
            for (int i = 0; i < buffs.Count; i++)
            {
                if (buffs[i].Effect == buff.Effect)
                {
                    buffs[i] += buff;
                    applied = true;
                    break;
                }
            }

            if (!applied)
            {
                buffs.Add(buff);
                effect.sprite = buff.Effect.OnFloorSprite;
            }
        }
        
        public override List<Cell> Neighbours
        { 
            get => neighbours;
            set => neighbours = value;
        }

        public override void FallIn()
        {
            if (!isUnderGround) return;
            isUnderGround = false;
            FreeTheCell();
            background.sprite = full;
        }

    #region Mark As

        public override void MarkAsReachable()
        {
            frame.color = Colors[EColor.reachable];
            fade.color = Colors[EColor.reachable] * Colors[EColor.transparency];
            element.color = Colors[EColor.elementFull];
        }
        
        public override void MarkAsUnReachable()
        {
            frame.color = Colors[EColor.unMark];
            fade.color = Colors[EColor.reachable] * Colors[EColor.transparency];
            element.color = Colors[EColor.elementShadow];
        }

        public override void MarkAsPath()
        {
            frame.color = Colors[EColor.path];
            fade.color = Colors[EColor.path] * Colors[EColor.transparency];
            element.color = Colors[EColor.path];
        }

        public override void MarkAsHighlighted()
        {
            frame.color = Colors[EColor.highlighted];
            fade.color = Colors[EColor.highlighted] * Colors[EColor.transparency];
            element.color = Colors[EColor.elementFull];
        }

        public override void UnMark()
        {
            frame.color = Colors[EColor.unMark];
            fade.color = Colors[EColor.none];
            element.color = Colors[EColor.elementShadow];
        }

        public override void MarkAsInteractable()
        {
            frame.color = Colors[EColor.enemy];
            fade.color = Colors[EColor.enemy] * Colors[EColor.transparency];
            element.color = Colors[EColor.elementFull];
        }

    #endregion

    #region IInfo

        public string GetInfoMain()
        {
            return $"{cellType}";
        }

        public string GetInfoLeft()
        {
            string str = "";
            if (isTaken)
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
                str += $"{_buff.InfoBuff()}\n";
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
    
    [Serializable]
    public class SavedCell
    {
        public float[] position = new float[3];
        public float[] offsetCoord = new float[2];
        public ETile type;
        public GridObjectSO gridObject = null;

        public SavedCell(Cell _toSave)
        {
            Vector3 _position = _toSave.transform.position;
            position[0] = _position.x;
            position[1] = _position.y;
            position[2] = _position.z;

            Vector2 _offsetCoord = _toSave.OffsetCoord;
            offsetCoord[0] = _offsetCoord.x;
            offsetCoord[1] = _offsetCoord.y;
                
            type = _toSave.CellType;

            if (_toSave.isTaken && _toSave.CurrentGridObject != null)
            {
                gridObject = _toSave.CurrentGridObject.GridObjectSO;
            }
        }
    }
}