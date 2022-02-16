﻿using System;
using System.Collections.Generic;
using System.Linq;
using _Instances;
using Buffs;
using DataBases;
using Resources.ToolTip.Scripts;
using UnityEngine;

namespace Cells
{
    public class TileIsometric : Isometric, IInfo
    {
        [SerializeField] private CellSO cellSO;

        public override bool isCorrupted { get; set; }
        public override List<Buff> Buffs { get; set; }
        public override CellSO CellSO { get => cellSO; set => cellSO = value; }
        public Sprite full { get; set; }
        public override List<Cell> Neighbours => neighbours;


        [Header("Unity Infos")]
        [SerializeField] private SpriteRenderer frame;
        [SerializeField] private SpriteRenderer fade;
        [SerializeField] public SpriteRenderer element;
        [SerializeField] private SpriteRenderer effect;
        [SerializeField] public SpriteRenderer background;        
        [SerializeField] private ColorSet colorSet;
        [SerializeField] private Sprite selectFrame;
        [SerializeField] private Sprite fullFrame;
        
        private Dictionary<EColor, Color> Colors = new Dictionary<EColor, Color>();
        private CellState state;
        public CellState State => state;

        public override void Initialize()
        {
            isCorrupted = false;
            Buffs = new List<Buff>();
            Colors = colorSet.GetColors();
            CellSO.Spawn(this);
            AutoSort();
            
            if(CellSO.BasicBuff.Effect != null)
                AddBuff(new Buff(CellSO.BasicBuff));
        }

        private void AutoSort()
        {
            int position = (int) (transform.position.y / (GetCellDimensions().y/2f));
            frame.sortingOrder = 300 - position;
            fade.sortingOrder = 200 - position;
            element.sortingOrder = 500 - position;
            effect.sortingOrder = 100 - position;
            background.sortingOrder = 0 - position;
        }

        public override bool IsUnderGround { get; set; }

        public override Vector3 GetCellDimensions()
        {
            return new Vector3(2,1.154f,0);
        }
        
        public override void AddBuff(Buff _buff)
        {
            if (_buff == null) return;
            Buff buff = new Buff(_buff);
            buff.onFloor = true;
            
            bool applied = false;
            
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].Effect == buff.Effect)
                {
                    Buffs[i] += buff;
                    applied = true;
                    break;
                }
            }

            if (!applied)
            {
                Buffs.Add(buff);
                effect.sprite = buff.Effect.OnFloorSprite;
            }
        }

        public override void OnEndTurn()
        {
            Buffs.ForEach(b =>
            {
                b.OnEndTurn(CurrentUnit);
            });

            List<Buff> newList = new List<Buff>(Buffs.Where(b => b.Effect != CellSO.BasicBuff.Effect && b.Effect != DataBase.Cell.CorruptionSO));

            foreach (Buff _buff in newList.Where(_buff => _buff.Duration <= 0))
            {
                if (CurrentUnit != null) _buff.Undo(CurrentUnit);
                Buffs.Remove(_buff);
            }

            effect.sprite = Buffs.Count <= 0 
                ? null 
                : Buffs[Buffs.Count - 1].Effect.OnFloorSprite;
        }

        public override void FallIn()
        {
            if (!IsUnderGround) return;
            FreeTheCell();
            background.sprite = full;
            IsUnderGround = false;
        }

        #region Mark As
    
        public struct CellState
        {
            public Sprite frame;
            public Color frameColor;
            public Color HighlightColor;
            public Color ElementColor;

            public CellState(Sprite _frame, Color _frameColor, Color _highlightColor)
            {
                frame = _frame;
                frameColor = _frameColor;
                HighlightColor = _highlightColor;
                ElementColor = _frameColor;
            }

            public CellState(CellState _state)
            {
                frame = _state.frame;
                frameColor = _state.frameColor;
                HighlightColor = _state.HighlightColor;
                ElementColor = _state.frameColor;
            }
        }

        public void MarkAs(CellState _state)
        {
            frame.color = _state.frameColor;
            fade.color = _state.HighlightColor;
            element.color = _state.ElementColor;
            frame.sprite = _state.frame;
        }
        
        public override void MarkAsReachable()
        {
            
            state = new CellState(selectFrame, Colors[EColor.reachable],
                Colors[EColor.reachable] * Colors[EColor.transparency]);
            MarkAs(state);
            
        }
        
        public override void MarkAsUnReachable()
        {
            state = new CellState(null, Colors[EColor.unMark],
                Colors[EColor.reachable] * Colors[EColor.transparency]);
            MarkAs(state);
        }

        public override void MarkAsPath()
        {
            
            state = new CellState(selectFrame, Colors[EColor.path],
                Colors[EColor.path] * Colors[EColor.transparency]);
            MarkAs(state);
        }

        public override void MarkAsEnemyCell()
        {
            state = new CellState(selectFrame, Colors[EColor.enemy],
                Colors[EColor.enemy] * Colors[EColor.transparency]);
            MarkAs(state);
        }

        public override void MarkAsHighlighted()
        {
            try
            {
                state = new CellState(fullFrame, Colors[EColor.highlighted],
                    Colors[EColor.highlighted] * Colors[EColor.transparency]);
                MarkAs(state);
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log(e.StackTrace);
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
            }
            
        }

        public override void UnMark()
        {
            state = new CellState(null, Colors[EColor.unMark],
                Colors[EColor.none]);
            MarkAs(state);
        }

        public override void MarkAsInteractable()
        {
            state = new CellState(selectFrame, Colors[EColor.usable],
                Colors[EColor.usable] * Colors[EColor.transparency]);
            MarkAs(state);
        }
        
        public override void MarkAsValue(Gradient gradient, float value, int max)
        {
            try
            {
                state = new CellState(null, Colors[EColor.unMark],
                gradient.Evaluate(Mathf.Clamp((value / Math.Max(0.01f,max)), 0f, 1f)) * Colors[EColor.transparency]);
                MarkAs(state);
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
            }
        }

    #endregion

    #region IInfo

        public string GetInfoMain()
        {
            return $"{CellSO.Type}";
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