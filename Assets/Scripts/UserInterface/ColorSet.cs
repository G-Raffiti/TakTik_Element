using System.Collections.Generic;
using _Instances;
using _ScriptableObject;
using Stats;
using UnityEngine;

namespace UserInterface
{
        
    public enum EColor {none, unMark, highlighted, reachable, path, transparency, enemy, ally, elementShadow, elementFull, TurnPoint}
    
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
        [SerializeField] private Color turnPoint;
        [SerializeField] private Sprite selectFrame;
        public Sprite SelectFrame => selectFrame;
        [SerializeField] private Sprite fullFrame;
        public Sprite FullFrame => fullFrame;


        public Dictionary<EColor, Color> GetColors()
        {
            Dictionary<EColor, Color> Colors = new Dictionary<EColor, Color>
            {
                {EColor.none, Color.clear},
                {EColor.unMark, unMarkColor},
                {EColor.highlighted, highlightedColor},
                {EColor.reachable, reachableColor},
                {EColor.path, pathColor},
                {EColor.transparency, transparency},
                {EColor.enemy, enemyColor},
                {EColor.ally, allyColor},
                {EColor.elementShadow, elementShadow},
                {EColor.elementFull, Color.white},
                {EColor.TurnPoint, turnPoint},
            };

            return Colors;
        }

        public static Dictionary<EAffix, Color> GetAffixColors()
        {
            Dictionary<EAffix, Color> Colors = new Dictionary<EAffix, Color>();
            foreach (KeyValuePair<EAffix,AffixSO> _dataAffix in DataBase.Affix.Affixes)
            {
                Colors.Add(_dataAffix.Key, _dataAffix.Value.Color);
            }

            return Colors;
        }
        

        public string HexColor(EColor color)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(GetColors()[color])}";
        }
        
        public string HexColor(EAffix _affix)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(GetAffixColors()[_affix])}";
        }
        
        public static string HexColor(Color color)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(color)}";
        }
    }
}