using System;
using Stats;
using UnityEngine;

namespace _ScriptableObject
{
    [Serializable]
    [CreateAssetMenu(fileName = "Element_", menuName = "Scriptable Object/New Element")]
    public class Element : ScriptableObject
    {
        [SerializeField] private string elementName;
        [SerializeField] private Color textColour;
        [SerializeField] private EElement type;
        [SerializeField] private Sprite icon;
        [SerializeField] private string symbol;

        public string Name => ColorName();
        public Color TextColour => textColour;
        public EElement Type => type;

        public string ElementName => elementName;
        public Sprite Icon => icon;
        public string Symbol => symbol;

        public Element()
        {
            elementName = "None";
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
            return $"<color=#{_hexColor}>{elementName}</color>";
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