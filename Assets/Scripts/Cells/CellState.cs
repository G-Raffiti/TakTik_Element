using UnityEngine;

namespace Cells
{
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
}