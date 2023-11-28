using Buffs;
using UnityEngine;

namespace Cells
{
    public enum ECellType
    {
        Water,
        Hole,
        Dirt,
        Grass,
        Lava,
        SpaceLeef,
    }
    [CreateAssetMenu(fileName = "CellType_", menuName = "Scriptable Object/new Cell")]
    public class CellSo : ScriptableObject
    {
        [SerializeField] private Sprite background;
        [SerializeField] private Sprite element;
        [SerializeField] private Sprite full;
        [SerializeField] private Buff basicBuff = null;
        
        [SerializeField] private ECellType tileType;
        [SerializeField] private bool isUnderground;

        public Buff BasicBuff => basicBuff;

        public bool IsUnderground => isUnderground;
        public Sprite Background => background;
        public Sprite Element => element;
        public ECellType Type => tileType;

        public void Spawn(Cell _tile)
        {
            _tile.IsUnderGround = isUnderground;
            _tile.background.sprite = background;
            _tile.element.sprite = element;
            _tile.Full = full;
        }
    }
}