using StateMachine.UnitGenerators;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(CustomUnitGenerator))]
    public class CustomUnitGeneratorHelper : UnityEditor.Editor
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