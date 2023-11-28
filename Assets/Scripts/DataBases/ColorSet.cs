using System.Collections.Generic;
using _Instances;
using Stats;
using UnityEngine;

namespace DataBases
{
        
    public enum EColor {None, UnMark, Highlighted, Usable, Reachable, Path, Transparency, Enemy, Ally, ElementShadow, ElementFull, TurnPoint, Buff, Debuff}
    
    [CreateAssetMenu(fileName = "ColorSet_", menuName = "Scriptable Object/UI/Color Set")]
    public class ColorSet : ScriptableObject
    {
        [SerializeField] private Color unMarkColor;
        [SerializeField] private Color highlightedColor;
        [SerializeField] private Color reachableColor;
        [SerializeField] private Color pathColor;
        [SerializeField] private Color allyColor;
        [SerializeField] private Color usableColor;
        [SerializeField] private Color enemyColor;
        [SerializeField] private Color transparency;
        [SerializeField] private Color elementShadow;
        [SerializeField] private Color turnPoint;
        [SerializeField] private Color buff;
        [SerializeField] private Color debuff;


        public Dictionary<EColor, Color> GetColors()
        {
            Dictionary<EColor, Color> _colors = new Dictionary<EColor, Color>
            {
                {EColor.None, Color.clear},
                {EColor.UnMark, unMarkColor},
                {EColor.Highlighted, highlightedColor},
                {EColor.Reachable, reachableColor},
                {EColor.Usable, usableColor},
                {EColor.Path, pathColor},
                {EColor.Transparency, transparency},
                {EColor.Enemy, enemyColor},
                {EColor.Ally, allyColor},
                {EColor.ElementShadow, elementShadow},
                {EColor.ElementFull, Color.white},
                {EColor.TurnPoint, turnPoint},
                {EColor.Buff, buff},
                {EColor.Debuff, debuff},
            };

            return _colors;
        }

        public static Dictionary<EAffix, Color> GetAffixColors()
        {
            Dictionary<EAffix, Color> _colors = new Dictionary<EAffix, Color>();
            foreach (KeyValuePair<EAffix,AffixSo> _dataAffix in DataBase.Affix.Affixes)
            {
                _colors.Add(_dataAffix.Key, _dataAffix.Value.Color);
            }

            return _colors;
        }
        

        public string HexColor(EColor _color)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(GetColors()[_color])}";
        }
        
        public string HexColor(EAffix _affix)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(GetAffixColors()[_affix])}";
        }
        
        public static string HexColor(Color _color)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(_color)}";
        }
    }
}