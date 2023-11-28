using System;
using GridObjects;
using UnityEngine;

namespace Cells
{
    [Serializable]
    public class SavedCell
    {
        public float[] position = new float[3];
        public float[] offsetCoord = new float[2];
        public CellSo type;
        public GridObjectSo gridObject = null;
        public bool isSpawn;

        public SavedCell(TileIsometric _toSave)
        {
            Vector3 _position = _toSave.transform.position;
            position[0] = _position.x;
            position[1] = _position.y;
            position[2] = _position.z;

            Vector2 _offsetCoord = _toSave.OffsetCoord;
            offsetCoord[0] = _offsetCoord.x;
            offsetCoord[1] = _offsetCoord.y;
                
            type = _toSave.CellSo;

            if (_toSave.IsTaken && _toSave.CurrentGridObject != null)
            {
                gridObject = _toSave.CurrentGridObject.GridObjectSo;
            }

            isSpawn = _toSave.isSpawnPlace;
        }
    }
}