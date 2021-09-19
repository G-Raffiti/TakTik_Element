﻿using System;
using Stats;
using UnityEngine;

namespace _ScriptableObject
{
    [Serializable]
    [CreateAssetMenu(fileName = "Element_", menuName = "Scriptable Object/New Element")]
    public class Element : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Color textColour;
        [SerializeField] private EElement type;

        public string Name => ColorName();
        public Color TextColour => textColour;
        public EElement Type => type;

        public Element()
        {
            name = "None";
            textColour = Color.white;
            type = EElement.None;
        }

        public static Element None()
        {
            return ScriptableObject.CreateInstance<Element>();
        }

        private string ColorName()
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(TextColour);
            return $"<color=#{_hexColor}>{name}</color>";
        }
    }

    [Serializable]
    public struct SaveElement
    {
        public string name;
        public float[] color;

        public SaveElement(Element _toSave)
        {
            name = _toSave.Name;
            color = new float[4]
                {_toSave.TextColour.r, _toSave.TextColour.g, _toSave.TextColour.b, _toSave.TextColour.a};
        }
    }
    
}