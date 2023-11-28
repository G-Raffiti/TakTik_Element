﻿using System;
using System.Collections.Generic;
using EndConditions;
using UnityEditor;
using UnityEngine;

namespace Cells
{
    [CreateAssetMenu(fileName = "Board_", menuName = "Scriptable Object/New Board")]
    public class BoardSo : ScriptableObject
    {
        [SerializeField] private Sprite background;
        public Sprite Background => background;
        
        [SerializeField] private List<SavedCell> cells = new List<SavedCell>();
        public List<SavedCell> Cells => cells;

        [SerializeField] private CameraSaved camera;
        public CameraSaved Camera => camera;
        
        [SerializeField] private EndConditionSo endCondition;
        public EndConditionSo EndCondition => endCondition;

        /// <summary>
        /// Modify a BoardSO to save the actual scene
        /// </summary>
        public void SaveBoard(List<SavedCell> _list, Sprite _background, Camera _camera)
        {
#if UNITY_EDITOR
            cells = new List<SavedCell>();
            
            foreach (SavedCell _cell in _list)
            {
                cells.Add(_cell);
            }

            background = _background;
            camera = new CameraSaved(_camera);
            
            EditorUtility.SetDirty(this); 
            AssetDatabase.SaveAssets(); 
            AssetDatabase.Refresh();
#endif
        }
    }

    [Serializable]
    public class CameraSaved
    {
        public float x;
        public float y;
        public float size;

        public CameraSaved(Camera _camera)
        {
            x = _camera.transform.position.x;
            y = _camera.transform.position.y;
            size = _camera.orthographicSize;
        }
    }
}