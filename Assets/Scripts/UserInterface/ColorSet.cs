using System.Collections.Generic;
using _ScriptableObject;
using UnityEngine;

namespace UserInterface
{
        
    public enum EColor {none, unMark, highlighted, reachable, path, transparency, enemy, ally, elementShadow, elementFull, HP, MP, AP, Speed, TurnPoint, fire, nature, water}
    
    [CreateAssetMenu(fileName = "ColorSet_", menuName = "Scriptable Object/UI/Color Set")]
    public class ColorSet : ScriptableObject
    {
        [SerializeField] private Color unMarkColor;
        [SerializeField] private Color highlightedColor;
        [SerializeField] private Color reachableColor;
        [SerializeField] private Color pathColor;
        [SerializeField] private Color allyColor;
        [SerializeField] private Color enemyColor;
        [SerializeField] private Color transparency;
        [SerializeField] private Color elementShadow;
        [SerializeField] private Color HP;
        [SerializeField] private Color MP;
        [SerializeField] private Color AP;
        [SerializeField] private Color Speed;
        [SerializeField] private Color TurnPoint;
        [SerializeField] private Element Fire;
        [SerializeField] private Element Nature;
        [SerializeField] private Element Water;


        public Dictionary<EColor, Color> GetColors()
        {
            Dictionary<EColor, Color> Colors = new Dictionary<EColor, Color>();
            Colors.Add(EColor.none, Color.clear);
            Colors.Add(EColor.unMark, unMarkColor);
            Colors.Add(EColor.highlighted, highlightedColor);
            Colors.Add(EColor.reachable, reachableColor);
            Colors.Add(EColor.path, pathColor);
            Colors.Add(EColor.transparency, transparency);
            Colors.Add(EColor.enemy, enemyColor);
            Colors.Add(EColor.ally, allyColor);
            Colors.Add(EColor.elementShadow, elementShadow);
            Colors.Add(EColor.elementFull, Color.white);
            Colors.Add(EColor.HP, HP);
            Colors.Add(EColor.MP, MP);
            Colors.Add(EColor.AP, AP);
            Colors.Add(EColor.Speed, Speed);
            Colors.Add(EColor.TurnPoint, TurnPoint);
            Colors.Add(EColor.fire, Fire.TextColour);
            Colors.Add(EColor.nature, Nature.TextColour);
            Colors.Add(EColor.water, Water.TextColour);

            return Colors;
        }

        public string HexColor(EColor color)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(GetColors()[color])}";
        }
        
        public static string HexColor(Color color)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(color)}";
        }
    }
}