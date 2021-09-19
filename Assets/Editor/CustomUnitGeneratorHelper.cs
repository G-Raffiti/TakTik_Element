using Grid.UnitGenerators;
using UnityEngine;
using UnityEditor;

namespace TbsFramework.EditorUtils
{
    [CustomEditor(typeof(CustomUnitGenerator))]
    public class CustomUnitGeneratorHelper : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CustomUnitGenerator unitGenerator = (CustomUnitGenerator)target;

            if (GUILayout.Button("Snap to Grid"))
            {
                unitGenerator.SnapToGrid();
            }
        }
    }
}