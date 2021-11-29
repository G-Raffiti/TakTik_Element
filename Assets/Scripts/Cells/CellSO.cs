using StatusEffect;
using UnityEngine;

namespace Cells
{
    [CreateAssetMenu(fileName = "CellType_", menuName = "Scriptable Object/new Cell")]
    public class CellSO : ScriptableObject
    {
        [SerializeField] private Sprite background;
        [SerializeField] private Sprite element;
        [SerializeField] private Sprite full;
        [SerializeField] private Buff basicBuff = null;
        
        [SerializeField] private string tileType;
        [SerializeField] private bool isUnderground;

        public Buff BasicBuff => basicBuff;

        public bool IsUnderground => isUnderground;
        public Sprite Background => background;
        public Sprite Element => element;
        public string Type => tileType;

        public void Spawn(TileIsometric tile)
        {
            tile.IsUnderGround = isUnderground;
            tile.background.sprite = background;
            tile.element.sprite = element;
            tile.full = full;
        }
    }
}